namespace InsureYouAI.Entities
{
    public class Category
    {
        public int CategoryId { get => field; set => field = value; } 
        public string CategoryName { get => field; set => field = value.Trim(); } = null!;
        public ICollection<Article> Articles { get; set; } = [];
    }
}
