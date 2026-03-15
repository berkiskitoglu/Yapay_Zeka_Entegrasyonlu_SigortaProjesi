namespace InsureYouAI.Entities
{
    public class PricingPlanItem
    {
        public int PricingPlanItemId { get => field; set => field = value; }
        public string Title { get => field; set => field = value.Trim(); } = null!;
        public int PricingPlanId { get; set; }
        public PricingPlan PricingPlan { get; set; } = null!;
    }
}
