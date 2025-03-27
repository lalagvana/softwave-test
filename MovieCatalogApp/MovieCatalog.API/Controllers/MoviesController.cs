using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieCatalog.API.DTOs;
using MovieCatalog.Domain.Interfaces;
using MovieCatalog.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieCatalog.API.Controllers
{
    /// <summary>
    /// Controller for handling movie-related operations
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieService _movieService;
        private readonly ILogger<MoviesController> _logger;
        private readonly IMapper _mapper;

        public MoviesController(IMovieService movieService, ILogger<MoviesController> logger, IMapper mapper)
        {
            _movieService = movieService ?? throw new ArgumentNullException(nameof(movieService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// Gets a paginated list of popular movies
        /// </summary>
        /// <param name="page">Page number (defaults to 1)</param>
        /// <returns>Paginated list of popular movies</returns>
        [HttpGet("popular")]
        [ProducesResponseType(typeof(PaginatedResponseDto<MovieDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPopularMovies([FromQuery] int page = 1)
        {
            if (page < 1)
            {
                return BadRequest("Page number must be greater than 0");
            }

            try
            {
                var result = await _movieService.GetPopularMoviesAsync(page);
                return Ok(MapToPaginatedResponse(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting popular movies for page {Page}", page);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "An error occurred while fetching popular movies");
            }
        }

        /// <summary>
        /// Gets a paginated list of top-rated movies
        /// </summary>
        /// <param name="page">Page number (defaults to 1)</param>
        /// <returns>Paginated list of top-rated movies</returns>
        [HttpGet("top-rated")]
        [ProducesResponseType(typeof(PaginatedResponseDto<MovieDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTopRatedMovies([FromQuery] int page = 1)
        {
            if (page < 1)
            {
                return BadRequest("Page number must be greater than 0");
            }

            try
            {
                var result = await _movieService.GetTopRatedMoviesAsync(page);
                return Ok(MapToPaginatedResponse(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting top rated movies for page {Page}", page);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "An error occurred while fetching top rated movies");
            }
        }

        /// <summary>
        /// Searches for movies based on various criteria
        /// </summary>
        /// <param name="query">Search query text</param>
        /// <param name="year">Release year filter</param>
        /// <param name="language">Language filter</param>
        /// <param name="sortBy">Sort field</param>
        /// <param name="voteAverageGte">Minimum vote average</param>
        /// <param name="withGenres">Genre ID filter</param>
        /// <param name="page">Page number (defaults to 1)</param>
        /// <returns>Paginated list of movies matching search criteria</returns>
        [HttpGet("search")]
        [ProducesResponseType(typeof(PaginatedResponseDto<MovieDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SearchMovies(
            [FromQuery] string? query,
            [FromQuery] int? year = null,
            [FromQuery] string? language = null,
            [FromQuery] string? sortBy = null,
            [FromQuery] decimal? voteAverageGte = null,
            [FromQuery] int? withGenres = null,
            [FromQuery] int page = 1)
        {
            if (page < 1)
            {
                return BadRequest("Page number must be greater than 0");
            }

            if (voteAverageGte.HasValue && (voteAverageGte < 0 || voteAverageGte > 10))
            {
                return BadRequest("Vote average must be between 0 and 10");
            }

            try
            {
                var result = await _movieService.SearchMoviesAsync(
                    query, year, language, sortBy, voteAverageGte, withGenres, page);
                return Ok(MapToPaginatedResponse(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching movies with query: {Query}, page: {Page}", query, page);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "An error occurred while searching for movies");
            }
        }

        /// <summary>
        /// Gets detailed information for a specific movie
        /// </summary>
        /// <param name="id">Movie ID</param>
        /// <returns>Detailed movie information</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(MovieDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetMovieById(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Movie ID must be greater than 0");
            }

            try
            {
                var movie = await _movieService.GetMovieDetailsAsync(id);

                if (movie == null)
                {
                    return NotFound($"Movie with ID {id} not found");
                }

                return Ok(_mapper.Map<MovieDetailDto>(movie));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting movie details for ID {MovieId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"An error occurred while fetching movie with ID {id}");
            }
        }

        /// <summary>
        /// Gets all available movie genres
        /// </summary>
        /// <returns>List of movie genres</returns>
        [HttpGet("genres")]
        [ProducesResponseType(typeof(List<GenreDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetGenres()
        {
            try
            {
                var genres = await _movieService.GetGenresAsync();
                return Ok(_mapper.Map<List<GenreDto>>(genres));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting genres");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "An error occurred while fetching genres");
            }
        }

        /// <summary>
        /// Maps a paginated domain result to a paginated response DTO
        /// </summary>
        private PaginatedResponseDto<MovieDto> MapToPaginatedResponse<Movie>(PaginatedResult<Movie> result)
        {
            return new PaginatedResponseDto<MovieDto>
            {
                Page = result.Page,
                TotalPages = result.TotalPages,
                TotalResults = result.TotalResults,
                Results = result.Results.Select(x => _mapper.Map<MovieDto>(x)).ToList()
            };
        }
    }
}
