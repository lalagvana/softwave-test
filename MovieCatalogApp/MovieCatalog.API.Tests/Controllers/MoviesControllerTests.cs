using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using MovieCatalog.API.Controllers;
using MovieCatalog.API.Mappers;
using MovieCatalog.Domain.Interfaces;
using MovieCatalog.Domain.Models;

namespace MovieCatalog.API.Tests.Controllers
{
    public class MoviesControllerTests
    {
        private readonly Mock<IMovieService> _mockMovieService;
        private readonly Mock<ILogger<MoviesController>> _mockLogger;
        private readonly Mock<IMapper> _mockMapper;
        private readonly MoviesController _controller;

        public MoviesControllerTests()
        {
            _mockMovieService = new Mock<IMovieService>();
            _mockLogger = new Mock<ILogger<MoviesController>>();
            _mockMapper = new Mock<IMapper>();
            _controller = new MoviesController(_mockMovieService.Object, _mockLogger.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task GetPopularMovies_ReturnsOkResult_WithPaginatedMovies()
        {
            // Arrange
            var expectedResult = new PaginatedResult<Movie>
            {
                Page = 1,
                TotalPages = 10,
                TotalResults = 200,
                Results = new List<Movie> { new Movie { Id = 1, Title = "Test Movie" } }
            };

            _mockMovieService.Setup(x => x.GetPopularMoviesAsync(It.IsAny<int>()))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.GetPopularMovies(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task GetMovieById_WithValidId_ReturnsOkResult()
        {
            // Arrange
            var movieId = 1;
            var movie = new Movie { Id = movieId, Title = "Test Movie" };

            _mockMovieService.Setup(x => x.GetMovieDetailsAsync(movieId))
                .ReturnsAsync(movie);

            // Act
            var result = await _controller.GetMovieById(movieId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task GetMovieById_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var movieId = 999;

            _mockMovieService.Setup(x => x.GetMovieDetailsAsync(movieId))
                .ReturnsAsync(null as Movie);

            // Act
            var result = await _controller.GetMovieById(movieId);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task SearchMovies_WithEmptyQuery_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.SearchMovies("");

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task SearchMovies_WithValidQuery_ReturnsOkResult()
        {
            // Arrange
            var query = "test";
            var expectedResult = new PaginatedResult<Movie>
            {
                Page = 1,
                TotalPages = 1,
                TotalResults = 1,
                Results = new List<Movie> { new Movie { Id = 1, Title = "Test Movie" } }
            };

            _mockMovieService.Setup(x => x.SearchMoviesAsync(
                    query, It.IsAny<int?>(), It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<decimal?>(), It.IsAny<int?>(), It.IsAny<int>()))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.SearchMovies(query);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }
    }
}
