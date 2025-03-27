using System.Text.Json.Serialization;

namespace MovieCatalog.Infrastructure.DTOs
{
    /// <summary>
    /// Represents a paginated response from the TMDB API
    /// </summary>
    /// <typeparam name="T">The type of items in the results collection</typeparam>
    public class TmdbPaginatedResponse<T>
    {
        /// <summary>
        /// Current page number
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Collection of items for the current page
        /// </summary>
        public List<T> Results { get; set; } = new();

        /// <summary>
        /// Total number of available pages
        /// </summary>
        [JsonPropertyName("total_pages")]
        public int TotalPages { get; set; }

        /// <summary>
        /// Total number of items across all pages
        /// </summary>
        [JsonPropertyName("total_results")]
        public int TotalResults { get; set; }
    }

    /// <summary>
    /// Represents basic movie information from TMDB API
    /// </summary>
    public class TmdbMovieDto
    {
        /// <summary>
        /// TMDB unique identifier for the movie
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Localized title of the movie
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Original title in the movie's native language
        /// </summary>
        [JsonPropertyName("original_title")]
        public string? OriginalTitle { get; set; }

        /// <summary>
        /// Movie plot summary
        /// </summary>
        public string? Overview { get; set; }

        /// <summary>
        /// Relative path to the movie's poster image
        /// </summary>
        [JsonPropertyName("poster_path")]
        public string? PosterPath { get; set; }

        /// <summary>
        /// Relative path to the movie's backdrop image
        /// </summary>
        [JsonPropertyName("backdrop_path")]
        public string? BackdropPath { get; set; }

        /// <summary>
        /// Average user rating (0-10)
        /// </summary>
        [JsonPropertyName("vote_average")]
        public decimal VoteAverage { get; set; }

        /// <summary>
        /// Number of user votes
        /// </summary>
        [JsonPropertyName("vote_count")]
        public int VoteCount { get; set; }

        /// <summary>
        /// Release date in ISO format (YYYY-MM-DD)
        /// </summary>
        [JsonPropertyName("release_date")]
        public string? ReleaseDate { get; set; }

        /// <summary>
        /// List of genre IDs associated with the movie
        /// </summary>
        [JsonPropertyName("genre_ids")]
        public List<int>? GenreIds { get; set; }

        /// <summary>
        /// Popularity score based on TMDB algorithm
        /// </summary>
        public decimal? Popularity { get; set; }

        /// <summary>
        /// Original language ISO code (e.g., "en" for English)
        /// </summary>
        [JsonPropertyName("original_language")]
        public string? OriginalLanguage { get; set; }

        /// <summary>
        /// Indicates if the movie is for adults only
        /// </summary>
        public bool Adult { get; set; }

        /// <summary>
        /// Indicates if the movie has associated video content
        /// </summary>
        public bool Video { get; set; }
    }

    /// <summary>
    /// Represents detailed movie information including credits and additional metadata
    /// </summary>
    public class TmdbMovieDetailDto : TmdbMovieDto
    {
        /// <summary>
        /// Complete list of genres associated with the movie
        /// </summary>
        public List<TmdbGenreDto>? Genres { get; set; }

        /// <summary>
        /// Production status (e.g., "Released", "In Production")
        /// </summary>
        public string? Status { get; set; }

        /// <summary>
        /// Movie runtime in minutes
        /// </summary>
        public int? Runtime { get; set; }

        /// <summary>
        /// Cast and crew information
        /// </summary>
        public TmdbCreditsDto? Credits { get; set; }
    }

    /// <summary>
    /// Represents a movie genre
    /// </summary>
    public class TmdbGenreDto
    {
        /// <summary>
        /// TMDB unique identifier for the genre
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Genre name
        /// </summary>
        public string Name { get; set; } = string.Empty;
    }

    /// <summary>
    /// Contains cast and crew information for a movie
    /// </summary>
    public class TmdbCreditsDto
    {
        /// <summary>
        /// List of cast members
        /// </summary>
        public List<TmdbCastDto>? Cast { get; set; }

        /// <summary>
        /// List of crew members
        /// </summary>
        public List<TmdbCrewDto>? Crew { get; set; }
    }

    /// <summary>
    /// Represents an actor in a movie
    /// </summary>
    public class TmdbCastDto
    {
        /// <summary>
        /// TMDB unique identifier for the person
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Actor's full name
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Character name played by the actor
        /// </summary>
        public string? Character { get; set; }

        /// <summary>
        /// Relative path to the actor's profile image
        /// </summary>
        [JsonPropertyName("profile_path")]
        public string? ProfilePath { get; set; }

        /// <summary>
        /// Cast order/billing position
        /// </summary>
        public int Order { get; set; }
    }

    /// <summary>
    /// Represents a crew member in a movie
    /// </summary>
    public class TmdbCrewDto
    {
        /// <summary>
        /// TMDB unique identifier for the person
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Crew member's full name
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Specific job title (e.g., "Director", "Screenplay")
        /// </summary>
        public string? Job { get; set; }

        /// <summary>
        /// Department (e.g., "Directing", "Writing")
        /// </summary>
        public string? Department { get; set; }

        /// <summary>
        /// Relative path to the crew member's profile image
        /// </summary>
        [JsonPropertyName("profile_path")]
        public string? ProfilePath { get; set; }
    }

    /// <summary>
    /// Response containing a list of all available genres
    /// </summary>
    public class TmdbGenresResponse
    {
        /// <summary>
        /// List of movie genres
        /// </summary>
        public List<TmdbGenreDto> Genres { get; set; } = new();
    }
}
