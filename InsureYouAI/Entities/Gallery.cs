namespace InsureYouAI.Entities
{
    public class Gallery
    {
        public int GalleryId { get => field; set => field = value; }
        public string Title { get => field; set => field = value.Trim(); } = null!;
        public string ImageUrl { get => field; set => field = value.Trim(); } = null!;
    }
}
