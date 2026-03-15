namespace InsureYouAI.Entities
{
    public class About
    {
        public int AboutId { get => field; set => field = value; }
        public string Title { get => field; set => field = value.Trim(); } = null!;
        public string Description { get => field; set => field = value.Trim(); } = null!;
        public string ImageUrl { get => field; set => field = value.Trim(); } = null!;
    }
}
