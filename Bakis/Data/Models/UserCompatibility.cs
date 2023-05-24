namespace Bakis.Data.Models
{
    public class UserCompatibility
    {
        public int Id { get; set; }
        public string UserId1 { get; set; }
        public string UserId2 { get; set; }
        public double Compatibility { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
