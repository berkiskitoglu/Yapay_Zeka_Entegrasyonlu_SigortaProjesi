namespace InsureYouAI.Entities
{
    public class Revenue
    {
        public int RevenueId { get => field; set => field = value; }
        public string NameSurname { get => field; set => field = value.Trim(); } = null!;
        public string Detail { get => field; set => field = value.Trim(); } = null!;
        public decimal Amount { get => field; set => field = value; }
        public DateTime ProcessDate { get => field; set => field = value == default ? DateTime.UtcNow : value; }

    }
}
