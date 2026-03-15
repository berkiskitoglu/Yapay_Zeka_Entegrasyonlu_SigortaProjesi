namespace InsureYouAI.Entities
{
    public class Testimonial
    {
        public int TestimonialId { get => field; set => field = value; }
        public string NameSurname { get => field; set => field = value.Trim(); } = null!;
        public string Title { get => field; set => field = value.Trim(); } = null!;
        public string ImageUrl { get => field; set => field = value.Trim(); } = null!;
        public string CommentDetail { get => field; set => field = value.Trim(); } = null!;

    }
}
