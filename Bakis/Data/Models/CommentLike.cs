namespace Bakis.Data.Models
{
    public class CommentLike
    {
        public int Id { get; set; }
        public int CommentId { get; set; }
        public string Type { get; set; }
        public Comment? Comment { get; set; } = null;
    }
}
