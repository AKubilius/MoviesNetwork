namespace Bakis.Data.Models.DTOs
{
    public class ChallengeCreateDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Count { get; set; }
        public List<ChallengeCondition> Conditions { get; set; }
    }
}
