namespace InsureYouAI.Models
{
    public class ArticleListViewModel
    {
        public int ArticleId { get; set; }
        public string Title { get; set; } = null!;
        public string CategoryName { get; set; } = null!;
        public string Author { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
        public string ImageUrl { get; set; } = null!;
        public string Content { get; set; } = null!;
        public int CommentCount { get; set; }
    }
}
