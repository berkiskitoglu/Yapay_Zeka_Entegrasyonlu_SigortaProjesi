namespace InsureYouAI.Entities
{
    public class Contact
    {
        public int ContactId { get => field; set => field = value; }
        public string Description { get => field; set => field = value.Trim(); } = null!;
        public string Phone { get => field; set => field = value.Trim().Replace(" ",""); } = null!;
        public string Email { get => field; set => field = value.Trim().ToLower(); } = null!;
        public string Address { get => field; set => field = value.Trim(); } = null!;

    }
}
