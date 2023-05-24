namespace Bakis.Data.Models
{
    public class MovieDetailsComparer : IEqualityComparer<MovieDetails>
    {
        public bool Equals(MovieDetails x, MovieDetails y)
        {
            return x.Id == y.Id;
        }

        public int GetHashCode(MovieDetails obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}
