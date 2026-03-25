using Microsoft.ML;
using Microsoft.ML.Transforms.TimeSeries;

namespace InsureYouAI.Services
{
    public class PolicySalesData
    {
        public DateTime Date { get; set; }
        public float SaleCount { get; set; }
    }

    public class PolicySalesForecast
    {
        public float[] ForecastedValues { get; set; }
        public float[] LowerBoundValues { get; set; }
        public float[] UpperBoundValues { get; set; }
    }

    public class ForecastService
    {
        private readonly MLContext _mLContext;

        public ForecastService()
        {
            _mLContext = new MLContext();
        }

        public PolicySalesForecast GetForecast(List<PolicySalesData> salesData , int horizon = 3)
        {
            var dataView = _mLContext.Data.LoadFromEnumerable(salesData);
            var forecastingPipeline = _mLContext.Forecasting.ForecastBySsa(
                outputColumnName: "ForecastedValues",
                inputColumnName: "SaleCount",
                windowSize:3,
                seriesLength:6,
                trainSize:salesData.Count - horizon,
                horizon:horizon,
                confidenceLevel: 0.95f,
                confidenceLowerBoundColumn: "LowerBoundValues",
                confidenceUpperBoundColumn: "UpperBoundValues"
                );
            var model = forecastingPipeline.Fit(dataView);
            var forecastingEngine = model.CreateTimeSeriesEngine<PolicySalesData, PolicySalesForecast>(_mLContext);
            return forecastingEngine.Predict();
        }


    }
}
