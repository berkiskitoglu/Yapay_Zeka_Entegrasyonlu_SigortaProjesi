namespace InsureYouAI.Entities
{
    public class ClaudeAIMessage
    {
        public int ClaudeAIMessageId { get => field; set => field = value; }
        public string MessageDetail { get => field; set => field = value.Trim(); } = null!;
        public string ReceiverEmail { get => field; set => field = value.Trim(); } = null!;
        public string ReceiverNameSurname { get => field; set => field = value.Trim(); } = null!;
        public DateTime SendDate { get => field; set => field = value == default ? DateTime.UtcNow : value; }

    }
}
