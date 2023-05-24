namespace Bakis.Data.Models
{
    public class UserChallenge
    {
        public int Id { get; set; }
        public string? UserId { get; set; } = null;
        public int ChallengeId { get; set; }
        public int Progress { get; set; }
        public bool Completed { get; set; }

        public User? User { get; set; } = null;
        public Challenge? Challenge { get; set; } = null;
    }
}
