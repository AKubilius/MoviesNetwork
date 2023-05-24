using Bakis.Data;
using Bakis.Data.Models;
using IO.Ably;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Bakis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecommendationController : ControllerBase
    {
        private const string TmdbApiBase = "https://api.themoviedb.org/3";
        private readonly ApplicationDbContext _databaseContext;
        private readonly IAuthorizationService _authorizationService;
        private readonly HttpClient _httpClient;
        private readonly string API_KEY;
        public RecommendationController(IHttpClientFactory httpClientFactory, ApplicationDbContext context, IAuthorizationService authorizationService, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _databaseContext = context;
            _authorizationService = authorizationService;
            API_KEY = configuration["TMDB:ApiKey"];
        }

        public async Task<List<MovieDetails>> GetRecommendationsForUserIdsAsync(List<string> userIds, int page, int pageSize = 20)
        {
            var userRecommendations = new List<List<MovieDetails>>();
            userIds.Add(User.FindFirstValue(JwtRegisteredClaimNames.Sub));

            foreach (var userId in userIds)
            {
                var userMovies = await GetUserMovieIdsAsync(userId);
                var recommendations = new List<MovieDetails>();

                foreach (var movieId in userMovies)
                {
                    var movieRecommendations = await GetRecommendations(movieId, page);
                    recommendations.AddRange(movieRecommendations);
                }

                userRecommendations.Add(recommendations);
            }

          
            var commonMovies = GetCommonRecommendations(userRecommendations);

            return commonMovies
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        public async Task<List<int>> GetUserMovieIdsAsync(string userId)
        {
            var movieList = await _databaseContext.Lists
                .Where(list => list.UserId == userId)
                .ToListAsync();

            return movieList.Select(list => list.MovieID).ToList();
        }

        [HttpGet("recommendations")]
        public async Task<IActionResult> GetRecommendations([FromQuery] List<string> userIds, [FromQuery] int page = 1)
        {
            userIds.Remove("undefined");
            var recommendations = await GetRecommendationsForUserIdsAsync(userIds, page);
            return Ok(new { Recommendations = recommendations });
        }

        public async Task<List<MovieDetails>> GetRecommendationsForMultipleUsers(List<List<int>> userMovieLists, int page, int pageSize = 20)
        {
            var combinedMovieList = GetCombinedMovieList(userMovieLists);
            var userRecommendations = new List<List<MovieDetails>>();

            foreach (var movieId in combinedMovieList)
            {
                var recommendations = await GetRecommendations(movieId, page);
                userRecommendations.Add(recommendations);
            }

            
            var commonMovies = GetCommonRecommendations(userRecommendations);

            return commonMovies
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        private HashSet<int> GetCombinedMovieList(List<List<int>> userMovieLists)
        {
            var combinedList = new HashSet<int>();
            foreach (var list in userMovieLists)
            {
                foreach (var movieId in list)
                {
                    combinedList.Add(movieId);
                }
            }
            return combinedList;
        }

        private async Task<List<MovieDetails>> GetRecommendations(int movieId, int page)
        {
            var url = $"{TmdbApiBase}/movie/{movieId}/recommendations?api_key={API_KEY}&page={page}";
            var response = await _httpClient.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();

            dynamic json = JObject.Parse(content);
            var recommendations = new List<MovieDetails>();

            foreach (var movie in json.results)
            {
                var movieDetails = new MovieDetails
                {
                    Id = (int)movie.id,
                    Title = (string)movie.title,
                    backdrop_path = (string)movie.backdrop_path,
                    poster_path = (string)movie.poster_path,
                    release_date = (string)movie.release_date,
                    average_count = (string)movie.average_count
                };

                recommendations.Add(movieDetails);
            }

            return recommendations;
        }
        private List<MovieDetails> GetCommonRecommendations(List<List<MovieDetails>> userRecommendations)
        {
            return userRecommendations
                .Skip(1)
                .Aggregate(new HashSet<MovieDetails>(userRecommendations.First(), new MovieDetailsComparer()), (h, e) =>
                {
                    h.IntersectWith(e);
                    return h;
                })
                .ToList();
        }
    }
}
