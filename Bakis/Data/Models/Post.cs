using Bakis.Auth.Model;

namespace Bakis.Data.Models
{
    public class Post : IUserOwnedResource
    {
        public int Id { get; set; }
        public string Body { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ImageUrl { get; set; }
        public int MovieId { get; set; }
        public string? UserId { get; set; } = null;
        public User? User { get; set; } = null;

    }
}
