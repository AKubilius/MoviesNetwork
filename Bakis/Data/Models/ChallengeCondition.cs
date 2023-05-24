namespace Bakis.Data.Models
{
    public class ChallengeCondition
    {
        public int Id { get; set; }
        public int ChallengeId { get; set; }
        public string Type { get; set; } // For example: "Genre", "Actor", or "Length"
        public string Value { get; set; } // Store the required value as a string, e.g., "Comedy", "Tom Hanks", or "120"
    }
}
