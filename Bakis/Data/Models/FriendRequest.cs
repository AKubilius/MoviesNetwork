namespace Bakis.Data.Models
{
    public class FriendRequest
    {
        public int Id { get; set; }
        public string? Name { get; set; } = null;
        public string? InvitedBy { get; set; } = null;
        public string FriendId { get; set; }

    }
}
