using Bakis.Auth.Model;

namespace Bakis.Data.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string SenderId { get; set; }
        public virtual User Sender { get; set; }
        public string Body { get; set; }
        public DateTime DateTime { get; set; }

        public Boolean IsMovie { get; set; } = false;

        public int RoomId { get; set; }
        public Room Room { get; set; }

    }
}
