namespace InsureYouAI.Entities
{
    public class Comment
    {
        public int CommentId { get => field; set => field = value; }
        public string CommentDetail { get => field; set => field = value.Trim(); } = null!;
        public DateTime CommentDate { get => field; set => field = value == default ? DateTime.UtcNow : value; }

        public AppUser AppUser { get; set; } = null!;
        public string AppUserId { get; set; } = null!;

        public int ArticleId { get; set; }
        public Article Article { get; set; } = null!;
        
    }
}
