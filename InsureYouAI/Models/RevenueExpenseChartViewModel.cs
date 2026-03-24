namespace InsureYouAI.Models
{
    public class RevenueExpenseChartViewModel
    {
        public IEnumerable<string>? Months { get; set; }
        public IEnumerable<decimal>? RevenueTotals { get; set; }
        public IEnumerable<decimal>? ExpenseTotals { get; set; }
    }
}
