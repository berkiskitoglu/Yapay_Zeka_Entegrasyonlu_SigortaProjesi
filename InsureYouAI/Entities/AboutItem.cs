namespace InsureYouAI.Entities
{
    public class AboutItem
    {
        public int AboutItemId { get => field; set => field = value; }
        public string Detail { get => field; set => field = value.Trim(); } = null!;
    }
}
