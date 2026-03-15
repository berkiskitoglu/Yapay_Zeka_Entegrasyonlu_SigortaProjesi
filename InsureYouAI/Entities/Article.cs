namespace InsureYouAI.Entities
{
    public class Article
    {
        public int ArticleId { get => field; set => field = value; }
        public string Title { get => field; set => field = value.Trim(); } = null!;
        public DateTime CreatedDate { get => field; set => field = value == default ? DateTime.UtcNow : value; }
        public string Content { get => field; set => field = value.Trim(); } = null!;
        public string CoverImageUrl { get => field; set => field = value.Trim(); } = null!;
        public string MainCoverImageUrl { get => field; set => field = value.Trim(); } = null!;
        public int CategoryId { get => field; set => field = value; }
        public Category Category { get; set; } = null!;
        public ICollection<Comment> Comments { get; set; } = null!;
    }
}
