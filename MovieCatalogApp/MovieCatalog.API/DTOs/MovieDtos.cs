using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MovieCatalog.API.DTOs
{
    /// <summary>
    /// Represents basic movie information for API responses
    /// </summary>
    public class MovieDto
    {
        /// <summary>
        /// Unique identifier for the movie
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Title of the movie
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Brief description of the movie's plot
        /// </summary>
        public string Overview { get; set; } = string.Empty;

        /// <summary>
        /// Relative path to the movie's poster image
        /// </summary>
        public string PosterPath { get; set; } = string.Empty;

        /// <summary>
        /// Average rating on a scale of 0-10
        /// </summary>
        public decimal VoteAverage { get; set; }

        /// <summary>
        /// Number of votes received
        /// </summary>
        public int VoteCount { get; set; }

        /// <summary>
        /// Date when the movie was released
        /// </summary>
        public DateTime? ReleaseDate { get; set; }

        /// <summary>
        /// Genres associated with the movie
        /// </summary>
        public List<GenreDto> Genres { get; set; } = new();

        /// <summary>
        /// Complete URL to the movie's poster image
        /// </summary>
        public string FullPosterPath { get; set; } = string.Empty;
    }

    /// <summary>
    /// Extended movie information including cast, crew, and additional details
    /// </summary>
    public class MovieDetailDto : MovieDto
    {
        /// <summary>
        /// Original title in the movie's native language
        /// </summary>
        public string OriginalTitle { get; set; } = string.Empty;

        /// <summary>
        /// Relative path to the movie's backdrop image
        /// </summary>
        public string BackdropPath { get; set; } = string.Empty;

        /// <summary>
        /// Complete URL to the movie's backdrop image
        /// </summary>
        public string FullBackdropPath { get; set; } = string.Empty;

        /// <summary>
        /// Production status (e.g., "Released", "In Production")
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Movie duration in minutes
        /// </summary>
        public int? Runtime { get; set; }

        /// <summary>
        /// Popularity score based on views, searches, etc.
        /// </summary>
        public decimal? Popularity { get; set; }

        /// <summary>
        /// Original language code (ISO 639-1)
        /// </summary>
        public string OriginalLanguage { get; set; } = string.Empty;

        /// <summary>
        /// List of actors appearing in the movie
        /// </summary>
        public List<CastDto> Cast { get; set; } = new();

        /// <summary>
        /// List of crew members involved in production
        /// </summary>
        public List<CrewDto> Crew { get; set; } = new();

        /// <summary>
        /// Indicates if the movie contains adult content
        /// </summary>
        public bool Adult { get; set; }
    }

    /// <summary>
    /// Represents a movie genre
    /// </summary>
    public class GenreDto
    {
        /// <summary>
        /// Unique identifier for the genre
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Name of the genre
        /// </summary>
        public string Name { get; set; } = string.Empty;
    }

    /// <summary>
    /// Represents an actor in a movie
    /// </summary>
    public class CastDto
    {
        /// <summary>
        /// Unique identifier for the person
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Actor's name
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Character played by the actor
        /// </summary>
        public string Character { get; set; } = string.Empty;

        /// <summary>
        /// Relative path to the actor's profile image
        /// </summary>
        public string ProfilePath { get; set; } = string.Empty;

        /// <summary>
        /// Complete URL to the actor's profile image
        /// </summary>
        public string FullProfilePath { get; set; } = string.Empty;

        /// <summary>
        /// Order of appearance in credits
        /// </summary>
        public int Order { get; set; }
    }

    /// <summary>
    /// Represents a crew member in a movie
    /// </summary>
    public class CrewDto
    {
        /// <summary>
        /// Unique identifier for the person
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Crew member's name
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Specific role (e.g., "Director", "Cinematographer")
        /// </summary>
        public string Job { get; set; } = string.Empty;

        /// <summary>
        /// Department they worked in (e.g., "Directing", "Camera")
        /// </summary>
        public string Department { get; set; } = string.Empty;

        /// <summary>
        /// Relative path to the crew member's profile image
        /// </summary>
        public string ProfilePath { get; set; } = string.Empty;

        /// <summary>
        /// Complete URL to the crew member's profile image
        /// </summary>
        public string FullProfilePath { get; set; } = string.Empty;
    }

    /// <summary>
    /// Generic paginated response container for any DTO type
    /// </summary>
    /// <typeparam name="T">Type of items in the results collection</typeparam>
    public class PaginatedResponseDto<T>
    {
        /// <summary>
        /// Current page number
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Total number of available pages
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// Total number of items across all pages
        /// </summary>
        public int TotalResults { get; set; }

        /// <summary>
        /// Collection of items for the current page
        /// </summary>
        public IEnumerable<T> Results { get; set; } = new List<T>();
    }
}
