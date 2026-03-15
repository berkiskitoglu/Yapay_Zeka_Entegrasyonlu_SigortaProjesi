namespace InsureYouAI.Entities
{
    public class Message
    {
        public int MessageId { get => field; set => field = value; }
        public string NameSurname { get => field; set => field = value.Trim(); } = null!;
        public string Subject { get => field; set => field = value.Trim(); } = null!;
        public string Email { get => field; set => field = value.Trim(); } = null!;
        public string MessageDetail { get => field; set => field = value.Trim(); } = null!;
        public DateTime SendDate { get => field; set => field = value == default ? DateTime.UtcNow : value; }
        public bool IsRead { get => field; set => field = value; }

    }
}
