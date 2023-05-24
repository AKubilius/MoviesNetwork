using Bakis.Auth.Model;

namespace Bakis.Data.Models
{
    public class Comment
    {
        public int Id { get; set; }
        

        public string Body { get; set; }

        public string? UserId { get; set; } = null;
        public User? User { get; set; } = null;

        public int PostId { get; set; }
        public Post? Post { get; set; } = null;

    }
}
