using Bakis.Auth.Model;

namespace Bakis.Data.Models
{
    public class MyList : IUserOwnedResource
    {
        public int ID { get; set; }
        public int MovieID { get; set; }

        public string? UserId { get; set; } = null;
        public User? User { get; set; } = null;
    }
}
