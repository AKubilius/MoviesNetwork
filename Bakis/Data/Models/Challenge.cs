namespace Bakis.Data.Models
{
    public class Challenge
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Count { get; set; }

        public List<ChallengeCondition> Conditions { get; set; }
    }
}
