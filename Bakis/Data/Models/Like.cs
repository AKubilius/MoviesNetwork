using Bakis.Auth.Model;

namespace Bakis.Data.Models
{
    public class Like
    {
        public int Id { get; set; }

        public string? Type { get; set; }
   
        public string? UserId { get; set; } = null;

        public int PostId { get; set; }
        public Post? Post { get; set; } = null;

        
    }
}
