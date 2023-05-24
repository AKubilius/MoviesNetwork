namespace Bakis.Data.Models.DTOs
{
    public class MovieDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Runtime { get; set; }
       
        public List<GenreDto> Genres { get; set; }
    }
}
