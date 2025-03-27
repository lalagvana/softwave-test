using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using MovieCatalog.API.DTOs;
using Xunit;

namespace MovieCatalog.API.IntegrationTests
{
    public class MoviesApiTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public MoviesApiTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task GetPopularMovies_ReturnsSuccessStatusCode()
        {
            // Act
            var response = await _client.GetAsync("/api/movies/popular");

            // Assert
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<PaginatedResponseDto<MovieDto>>();
            Assert.NotNull(result);
            Assert.True(result.Results.Any());
        }

        [Fact]
        public async Task GetTopRatedMovies_ReturnsSuccessStatusCode()
        {
            // Act
            var response = await _client.GetAsync("/api/movies/top-rated");

            // Assert
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<PaginatedResponseDto<MovieDto>>();
            Assert.NotNull(result);
            Assert.True(result.Results.Any());
        }

        [Fact]
        public async Task GetMovieById_WithValidId_ReturnsMovie()
        {
            // Arrange
            var validMovieId = 550; // Fight Club (a movie that should exist in TMDB)

            // Act
            var response = await _client.GetAsync($"/api/movies/{validMovieId}");

            // Assert
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<MovieDetailDto>();
            Assert.NotNull(result);
            Assert.Equal(validMovieId, result.Id);
        }

        [Fact]
        public async Task GetMovieById_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var invalidMovieId = -1;

            // Act
            var response = await _client.GetAsync($"/api/movies/{invalidMovieId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task SearchMovies_WithValidQuery_ReturnsResults()
        {
            // Arrange
            var query = "matrix";

            // Act
            var response = await _client.GetAsync($"/api/movies/search?query={query}");

            // Assert
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<PaginatedResponseDto<MovieDto>>();
            Assert.NotNull(result);
        }

        [Fact]
        public async Task SearchMovies_WithEmptyQuery_ReturnsBadRequest()
        {
            // Act
            var response = await _client.GetAsync("/api/movies/search?query=");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
