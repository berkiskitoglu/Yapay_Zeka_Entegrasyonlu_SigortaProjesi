using InsureYouAI.Context;
using InsureYouAI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ML;
using Microsoft.ML.Transforms.TimeSeries;

namespace InsureYouAI.ViewComponents.DashboardViewComponents
{
    public class _DashboardForecastingPoliciesSalesComponentPartial : ViewComponent
    {
        private readonly InsureContext _context;

        public _DashboardForecastingPoliciesSalesComponentPartial(InsureContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            // 🔥 Son 6 ayı dinamik al
            var months = Enumerable.Range(0, 6)
                .Select(i => DateTime.Now.AddMonths(-5 + i))
                .ToList();

            var startDate = months.First();
            var endDate = months.Last();

            // 🔥 Veri çek
            var rawData = _context.Policies
                .Where(x => x.CreatedDate >= startDate && x.CreatedDate <= endDate)
                .GroupBy(x => x.PolicyType)
                .Select(g => new
                {
                    PolicyType = g.Key,
                    MonthlyCounts = g
                        .GroupBy(z => new { z.CreatedDate.Year, z.CreatedDate.Month })
                        .Select(s => new
                        {
                            Year = s.Key.Year,
                            Month = s.Key.Month,
                            Count = s.Count()
                        })
                        .OrderBy(s => s.Year)
                        .ThenBy(s => s.Month)
                        .ToList()
                })
                .ToList();

            var ml = new MLContext();
            List<PolicyForecastViewModel> result = new();

            foreach (var item in rawData)
            {
                // 🔥 Eksik ayları doldur (çok kritik)
                var filledData = months.Select(m =>
                {
                    var existing = item.MonthlyCounts
                        .FirstOrDefault(x => x.Month == m.Month && x.Year == m.Year);

                    return existing != null ? existing.Count : 0;
                }).ToList();

                // 🔥 Yetersiz veri kontrolü
                if (filledData.Count < 6)
                    continue;

                // 🔥 ML input (index sıralı olmalı)
                int index = 0;
                var mlData = filledData.Select(val => new PolicyMonthlyData
                {
                    MonthIndex = index++,
                    Value = val
                });

                var dataView = ml.Data.LoadFromEnumerable(mlData);

                var pipeline = ml.Forecasting.ForecastBySsa(
                    outputColumnName: "Forecast",
                    inputColumnName: "Value",
                    windowSize: 2,
                    seriesLength: 6,
                    trainSize: 6,
                    horizon: 1);

                var model = pipeline.Fit(dataView);

                var forecastEngine = model.CreateTimeSeriesEngine<PolicyMonthlyData, PolicyForecastOutput>(ml);

                var prediction = forecastEngine.Predict();

                // 🔥 Negatif koruma
                int predicted = Math.Max(0, (int)prediction.Forecast[0]);

                result.Add(new PolicyForecastViewModel
                {
                    PolicyType = item.PolicyType,
                    ForecastCount = predicted
                });
            }

            // 🔥 Yüzde hesaplama
            int total = result.Sum(x => x.ForecastCount);

            foreach (var item in result)
                item.Percentage = total > 0 ? (item.ForecastCount * 100 / total) : 0;

            return View(result);
        }
    }

    public class PolicyMonthlyData
    {
        public float MonthIndex { get; set; }
        public float Value { get; set; }
    }

    public class PolicyForecastOutput
    {
        public float[] Forecast { get; set; }
    }
}