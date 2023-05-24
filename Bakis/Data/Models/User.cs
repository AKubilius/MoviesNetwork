using Microsoft.AspNetCore.Identity;

namespace Bakis.Data.Models
{
    public class User:IdentityUser
    {
        [PersonalData]
        public string? Name { get; set; }

        [PersonalData]
        public string? Surname { get; set; }
        [PersonalData]
        public DateTime? BirthDate { get; set; }
        [PersonalData]
        public int? Age { get; set; }

        public string? ProfileImageBase64 { get; set; }

        public List<UserRoom> UserRooms { get; set; }
        public virtual ICollection<Message> SentMessages { get; set; }
    }
}
