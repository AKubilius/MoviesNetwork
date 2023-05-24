using Bakis.Auth.Model;

namespace Bakis.Data.Models.DTOs
{
    public class MessageDto
    {
        public int Id { get; set; }
        public string Body { get; set; }
        public int RoomId { get; set; }
        public Boolean IsMovie { get; set; }
        public DateTime DateTime { get; set; }
        public UserDto Sender { get; set; }
    }
}
