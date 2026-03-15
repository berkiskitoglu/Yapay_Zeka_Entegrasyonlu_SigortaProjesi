using Microsoft.AspNetCore.Identity;

namespace InsureYouAI.Entities
{
    public class AppUser : IdentityUser
    {
        public string Name { get => field; set => field = value.Trim(); } = null!;
        public string Surname { get => field; set => field = value.Trim(); } = null!;
        public string ImageUrl { get => field; set => field = value.Trim(); } = null!;
        public string Description { get => field; set => field = value.Trim(); } = null!;
        public ICollection<Comment> Comments { get; set; } = null!;
    }
}
