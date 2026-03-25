namespace InsureYouAI.Entities
{
    public class Policy
    {
        public int PolicyId { get => field; set => field = value; }
        public string PolicyNumber { get => field; set => field = value.Trim(); } = null!;
        public string PolicyType { get => field; set => field = value.Trim(); } = null!;
        public decimal PremiumAmount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get => field; set => field = value.Trim(); } = "Active";
        public DateTime CreatedDate { get => field; set => field = value == default ? DateTime.UtcNow : value; }
        public DateTime UpdatedDate { get => field; set => field = value == default ? DateTime.UtcNow : value; }

        // Navigation Property
        public string AppUserId { get; set; } = null!;
        public AppUser AppUser { get; set; } = null!;
    }
}
