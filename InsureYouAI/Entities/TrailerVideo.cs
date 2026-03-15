namespace InsureYouAI.Entities
{
    public class TrailerVideo
    {
        public int TrailerVideoId { get => field; set => field = value; }
        public string Title { get => field; set => field = value.Trim(); } = null!;
        public string CoverImageUrl { get => field; set => field = value.Trim(); } = null!;
        public string VideoUrl { get => field; set => field = value.Trim(); } = null!;
    }
}
