namespace InsureYouAI.Entities
{
    public class Service
    {
        public int ServiceId { get => field; set => field = value; }
        public string Title { get => field; set => field = value.Trim(); } = null!;
        public string Description { get => field; set => field = value.Trim(); } = null!;
        public string IconUrl { get => field; set => field = value.Trim(); } = null!;
        public string ImageUrl { get => field; set => field = value.Trim(); } = null!;
    }
}
