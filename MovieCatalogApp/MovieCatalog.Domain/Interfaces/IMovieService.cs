using MovieCatalog.Domain.Models;

namespace MovieCatalog.Domain.Interfaces
{
    /// <summary>
    /// Service interface for movie-related operations
    /// </summary>
    public interface IMovieService
    {
        /// <summary>
        /// Retrieves a paginated list of popular movies
        /// </summary>
        /// <param name="page">Page number to retrieve (1-based)</param>
        /// <param name="cancellationToken">Optional cancellation token</param>
        /// <returns>Paginated result containing movies</returns>
        Task<PaginatedResult<Movie>> GetPopularMoviesAsync(
            int page = 1,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a paginated list of top-rated movies
        /// </summary>
        /// <param name="page">Page number to retrieve (1-based)</param>
        /// <param name="cancellationToken">Optional cancellation token</param>
        /// <returns>Paginated result containing movies</returns>
        Task<PaginatedResult<Movie>> GetTopRatedMoviesAsync(
            int page = 1,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Searches for movies using various filter criteria
        /// </summary>
        /// <param name="query">Search term for movie titles</param>
        /// <param name="year">Filter by release year</param>
        /// <param name="language">Filter by language (ISO 639-1 code)</param>
        /// <param name="sortBy">Sort results by specified field</param>
        /// <param name="voteAverageGte">Minimum vote average threshold</param>
        /// <param name="withGenres">Filter by genre ID</param>
        /// <param name="page">Page number to retrieve (1-based)</param>
        /// <param name="cancellationToken">Optional cancellation token</param>
        /// <returns>Paginated result containing movies matching the criteria</returns>
        Task<PaginatedResult<Movie>> SearchMoviesAsync(
            string? query,
            int? year = null,
            string? language = null,
            string? sortBy = null,
            decimal? voteAverageGte = null,
            int? withGenres = null,
            int page = 1,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves detailed information for a specific movie
        /// </summary>
        /// <param name="id">Movie ID</param>
        /// <param name="cancellationToken">Optional cancellation token</param>
        /// <returns>Movie details or null if not found</returns>
        Task<Movie?> GetMovieDetailsAsync(
            int id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves the list of available movie genres
        /// </summary>
        /// <param name="cancellationToken">Optional cancellation token</param>
        /// <returns>List of genres</returns>
        Task<List<Genre>> GetGenresAsync(
            CancellationToken cancellationToken = default);
    }
}
