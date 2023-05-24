namespace Bakis.Data.Models
{

    public enum Status
    {
        Delivered,
        Accepted,
        Declined
    }

    public class WatchingRequest
    {
        public int Id { get; set; }
        public Status Status { get; set; } = Status.Delivered;
        public string? InvitedById { get; set; } = null;
        public string FriendId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime WatchingDate { get; set; }
        public int MessageId { get; set; }

        public User? InvitedBy { get; set; } = null;
        public User? Friend { get; set; } = null;
    }
}
