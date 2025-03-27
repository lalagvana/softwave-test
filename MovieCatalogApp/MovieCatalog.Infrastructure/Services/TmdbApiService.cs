using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MovieCatalog.Domain.Interfaces;
using MovieCatalog.Domain.Models;
using MovieCatalog.Infrastructure.Configuration;
using MovieCatalog.Infrastructure.DTOs;

namespace MovieCatalog.Infrastructure.Services
{
    /// <summary>
    /// Implementation of IMovieService that fetches data from The Movie Database (TMDB) API
    /// </summary>
    public class TmdbApiService : IMovieService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private readonly ILogger<TmdbApiService> _logger;
        private readonly TmdbOptions _options;
        private readonly JsonSerializerOptions _jsonOptions;

        public TmdbApiService(
            HttpClient httpClient,
            IMemoryCache cache,
            ILogger<TmdbApiService> logger,
            IOptions<TmdbOptions> options)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));

            _httpClient.BaseAddress = new Uri(_options.BaseUrl);
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        /// <inheritdoc />
        public async Task<PaginatedResult<Movie>> GetPopularMoviesAsync(
            int page = 1,
            CancellationToken cancellationToken = default)
        {
            string cacheKey = $"popular_movies_page_{page}";

            if (_cache.TryGetValue(cacheKey, out PaginatedResult<Movie>? cachedResult) && cachedResult != null)
            {
                _logger.LogDebug("Cache hit for popular movies page {Page}", page);
                return cachedResult;
            }

            try
            {
                _logger.LogDebug("Fetching popular movies page {Page} from TMDB API", page);

                var response = await _httpClient.GetFromJsonAsync<TmdbPaginatedResponse<TmdbMovieDto>>(
                    $"movie/popular?api_key={_options.ApiKey}&page={page}",
                    _jsonOptions,
                    cancellationToken);

                if (response == null)
                {
                    throw new InvalidOperationException("Failed to get popular movies from TMDB");
                }

                var result = await MapToPaginatedResultAsync(response, cancellationToken);

                _cache.Set(cacheKey, result, TimeSpan.FromMinutes(_options.CacheExpirationMinutes));

                return result;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Network error while fetching popular movies: {Message}", ex.Message);
                throw;
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Request for popular movies was canceled");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching popular movies from TMDB API");
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<PaginatedResult<Movie>> GetTopRatedMoviesAsync(
            int page = 1,
            CancellationToken cancellationToken = default)
        {
            string cacheKey = $"top_rated_movies_page_{page}";

            if (_cache.TryGetValue(cacheKey, out PaginatedResult<Movie>? cachedResult) && cachedResult != null)
            {
                _logger.LogDebug("Cache hit for top rated movies page {Page}", page);
                return cachedResult;
            }

            try
            {
                _logger.LogDebug("Fetching top rated movies page {Page} from TMDB API", page);

                var response = await _httpClient.GetFromJsonAsync<TmdbPaginatedResponse<TmdbMovieDto>>(
                    $"movie/top_rated?api_key={_options.ApiKey}&page={page}",
                    _jsonOptions,
                    cancellationToken);

                if (response == null)
                {
                    throw new InvalidOperationException("Failed to get top rated movies from TMDB");
                }

                var result = await MapToPaginatedResultAsync(response, cancellationToken);

                _cache.Set(cacheKey, result, TimeSpan.FromMinutes(_options.CacheExpirationMinutes));

                return result;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Network error while fetching top rated movies: {Message}", ex.Message);
                throw;
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Request for top rated movies was canceled");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching top rated movies from TMDB API");
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<PaginatedResult<Movie>> SearchMoviesAsync(
            string? query,
            int? year = null,
            string? language = null,
            string? sortBy = null,
            decimal? voteAverageGte = null,
            int? withGenres = null,
            int page = 1,
            CancellationToken cancellationToken = default)
        {
            // Determine if we're doing a search or discover API call
            string apiEndpoint = !string.IsNullOrWhiteSpace(query) ? "search/movie" : "discover/movie";

            var queryParams = BuildSearchQueryParams(query, year, language, sortBy, voteAverageGte, withGenres, page);
            string queryString = string.Join("&", queryParams);
            string cacheKey = $"{apiEndpoint}_{queryString}";

            if (_cache.TryGetValue(cacheKey, out PaginatedResult<Movie>? cachedResult) && cachedResult != null)
            {
                _logger.LogDebug("Cache hit for movie search with params: {QueryString}", queryString);
                return cachedResult;
            }

            try
            {
                _logger.LogDebug("Searching movies with params: {QueryString}", queryString);

                var response = await _httpClient.GetFromJsonAsync<TmdbPaginatedResponse<TmdbMovieDto>>(
                    $"{apiEndpoint}?api_key={_options.ApiKey}&{queryString}",
                    _jsonOptions,
                    cancellationToken);

                if (response == null)
                {
                    throw new InvalidOperationException("Failed to search movies from TMDB");
                }

                var result = await MapToPaginatedResultAsync(response, cancellationToken);

                _cache.Set(cacheKey, result, TimeSpan.FromMinutes(_options.CacheExpirationMinutes));

                return result;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Network error while searching movies: {Message}", ex.Message);
                throw;
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Movie search request was canceled");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching movies from TMDB API");
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<Movie?> GetMovieDetailsAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            string cacheKey = $"movie_details_{id}";

            if (_cache.TryGetValue(cacheKey, out Movie? cachedMovie) && cachedMovie != null)
            {
                _logger.LogDebug("Cache hit for movie details ID {MovieId}", id);
                return cachedMovie;
            }

            try
            {
                _logger.LogDebug("Fetching movie details for ID {MovieId}", id);

                var movieDto = await _httpClient.GetFromJsonAsync<TmdbMovieDetailDto>(
                    $"movie/{id}?api_key={_options.ApiKey}&append_to_response=credits",
                    _jsonOptions,
                    cancellationToken);

                if (movieDto == null)
                {
                    return null;
                }

                var movie = MapToMovieDetail(movieDto);

                _cache.Set(cacheKey, movie, TimeSpan.FromMinutes(_options.CacheExpirationMinutes));

                return movie;
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogWarning("Movie with ID {MovieId} not found", id);
                return null;
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Request for movie details ID {MovieId} was canceled", id);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching movie details for ID {MovieId}", id);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<List<Genre>> GetGenresAsync(CancellationToken cancellationToken = default)
        {
            string cacheKey = "movie_genres";

            if (_cache.TryGetValue(cacheKey, out List<Genre>? cachedGenres) && cachedGenres != null)
            {
                _logger.LogDebug("Cache hit for movie genres");
                return cachedGenres;
            }

            try
            {
                _logger.LogDebug("Fetching movie genres from TMDB API");

                var response = await _httpClient.GetFromJsonAsync<TmdbGenresResponse>(
                    $"genre/movie/list?api_key={_options.ApiKey}",
                    _jsonOptions,
                    cancellationToken);

                if (response?.Genres == null)
                {
                    throw new InvalidOperationException("Failed to get genres from TMDB");
                }

                var genres = response.Genres.Select(g => new Genre
                {
                    Id = g.Id,
                    Name = g.Name
                }).ToList();

                // Genres change rarely, so cache them for a longer time
                _cache.Set(cacheKey, genres, TimeSpan.FromDays(1));

                return genres;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Network error while fetching genres: {Message}", ex.Message);
                throw;
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Request for movie genres was canceled");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching genres from TMDB API");
                throw;
            }
        }

        /// <summary>
        /// Builds query parameters for movie search/discover
        /// </summary>
        private static List<string> BuildSearchQueryParams(
            string? query,
            int? year,
            string? language,
            string? sortBy,
            decimal? voteAverageGte,
            int? withGenres,
            int page)
        {
            var queryParams = new List<string> { $"page={page}" };

            if (!string.IsNullOrWhiteSpace(query))
            {
                queryParams.Add($"query={Uri.EscapeDataString(query)}");
            }

            if (year.HasValue)
            {
                queryParams.Add($"primary_release_year={year.Value}");
            }

            if (!string.IsNullOrEmpty(language))
            {
                queryParams.Add($"with_original_language={language}");
            }

            if (!string.IsNullOrEmpty(sortBy))
            {
                queryParams.Add($"sort_by={sortBy}");
            }

            if (voteAverageGte.HasValue)
            {
                queryParams.Add($"vote_average.gte={voteAverageGte.Value}");
            }

            if (withGenres.HasValue)
            {
                queryParams.Add($"with_genres={withGenres.Value}");
            }

            return queryParams;
        }

        /// <summary>
        /// Maps TMDB paginated response to domain model
        /// </summary>
        private async Task<PaginatedResult<Movie>> MapToPaginatedResultAsync(
            TmdbPaginatedResponse<TmdbMovieDto> response,
            CancellationToken cancellationToken)
        {
            var genres = await GetGenresAsync(cancellationToken);
            var genreDictionary = genres.ToDictionary(g => g.Id, g => g.Name);

            return new PaginatedResult<Movie>
            {
                Page = response.Page,
                TotalPages = response.TotalPages,
                TotalResults = response.TotalResults,
                Results = response.Results.Select(dto => MapToMovie(dto, genreDictionary)).ToList()
            };
        }

        /// <summary>
        /// Maps TMDB movie DTO to domain model
        /// </summary>
        private static Movie MapToMovie(TmdbMovieDto dto, Dictionary<int, string> genreDictionary)
        {
            return new Movie
            {
                Id = dto.Id,
                Title = dto.Title,
                OriginalTitle = dto.OriginalTitle,
                Overview = dto.Overview ?? string.Empty,
                PosterPath = dto.PosterPath ?? string.Empty,
                BackdropPath = dto.BackdropPath ?? string.Empty,
                VoteAverage = dto.VoteAverage,
                VoteCount = dto.VoteCount,
                ReleaseDate = string.IsNullOrEmpty(dto.ReleaseDate) ? null :
                    DateTime.TryParse(dto.ReleaseDate, out var date) ? date : null,
                Genres = dto.GenreIds?.Select(id => new Genre
                {
                    Id = id,
                    Name = genreDictionary.GetValueOrDefault(id, "Unknown")
                }).ToList() ?? new List<Genre>(),
                Popularity = dto.Popularity,
                OriginalLanguage = dto.OriginalLanguage,
                Adult = dto.Adult,
                Video = dto.Video
            };
        }

        /// <summary>
        /// Maps TMDB movie detail DTO to domain model
        /// </summary>
        private static Movie MapToMovieDetail(TmdbMovieDetailDto dto)
        {
            var movie = new Movie
            {
                Id = dto.Id,
                Title = dto.Title,
                OriginalTitle = dto.OriginalTitle,
                Overview = dto.Overview ?? string.Empty,
                PosterPath = dto.PosterPath ?? string.Empty,
                BackdropPath = dto.BackdropPath ?? string.Empty,
                VoteAverage = dto.VoteAverage,
                VoteCount = dto.VoteCount,
                ReleaseDate = string.IsNullOrEmpty(dto.ReleaseDate) ? null :
                    DateTime.TryParse(dto.ReleaseDate, out var date) ? date : null,
                Genres = dto.Genres?.Select(g => new Genre
                {
                    Id = g.Id,
                    Name = g.Name
                }).ToList() ?? new List<Genre>(),
                Status = dto.Status ?? string.Empty,
                Runtime = dto.Runtime,
                Popularity = dto.Popularity,
                OriginalLanguage = dto.OriginalLanguage,
                Adult = dto.Adult,
                Video = dto.Video
            };

            if (dto.Credits != null)
            {
                movie.Cast = dto.Credits.Cast?.Select(c => new Cast
                {
                    Id = c.Id,
                    Name = c.Name,
                    Character = c.Character ?? string.Empty,
                    ProfilePath = c.ProfilePath ?? string.Empty,
                    Order = c.Order
                }).ToList() ?? new List<Cast>();

                movie.Crew = dto.Credits.Crew?.Select(c => new Crew
                {
                    Id = c.Id,
                    Name = c.Name,
                    Job = c.Job ?? string.Empty,
                    Department = c.Department ?? string.Empty,
                    ProfilePath = c.ProfilePath ?? string.Empty
                }).ToList() ?? new List<Crew>();
            }

            return movie;
        }
    }
}
