namespace InsureYouAI.Entities
{
    public class PricingPlan
    {
        public int PricingPlanId { get => field; set => field = value; }
        public string Title { get => field; set => field = value.Trim(); } = null!;
        public string Price { get => field; set => field = value.Trim(); } = null!;
        public bool  IsFeatured{ get => field; set => field = value; }
        public ICollection<PricingPlanItem> PricingPlanItems { get; set; } = null!;

    }
}
