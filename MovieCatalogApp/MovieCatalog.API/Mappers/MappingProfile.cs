using AutoMapper;
using MovieCatalog.API.DTOs;
using MovieCatalog.Domain.Models;

namespace MovieCatalog.API.Mappers
{
    /// <summary>
    /// Configures AutoMapper mappings between domain models and DTOs
    /// </summary>
    public class MappingProfile : Profile
    {
        private const string BaseImageUrl = "https://image.tmdb.org/t/p/";

        /// <summary>
        /// Standard poster size for movie thumbnails and details
        /// </summary>
        private const string PosterSize = "w500";

        /// <summary>
        /// Full size for backdrop images
        /// </summary>
        private const string BackdropSize = "original";

        /// <summary>
        /// Standard size for profile images
        /// </summary>
        private const string ProfileSize = "w185";

        public MappingProfile()
        {
            ConfigureMovieMappings();
            ConfigurePersonMappings();
            ConfigureGenreMappings();
        }

        /// <summary>
        /// Configures mappings for movie-related entities
        /// </summary>
        private void ConfigureMovieMappings()
        {
            // Basic movie mapping
            CreateMap<Movie, MovieDto>()
                .ForMember(
                    dest => dest.FullPosterPath,
                    opt => opt.MapFrom(src => GetFullImagePath(src.PosterPath, PosterSize)));

            // Detailed movie mapping (inherits from basic movie mapping)
            CreateMap<Movie, MovieDetailDto>()
                .ForMember(
                    dest => dest.FullPosterPath,
                    opt => opt.MapFrom(src => GetFullImagePath(src.PosterPath, PosterSize)))
                .ForMember(
                    dest => dest.FullBackdropPath,
                    opt => opt.MapFrom(src => GetFullImagePath(src.BackdropPath, BackdropSize)));
        }

        /// <summary>
        /// Configures mappings for person-related entities (cast and crew)
        /// </summary>
        private void ConfigurePersonMappings()
        {
            // Cast member mapping
            CreateMap<Cast, CastDto>()
                .ForMember(
                    dest => dest.FullProfilePath,
                    opt => opt.MapFrom(src => GetFullImagePath(src.ProfilePath, ProfileSize)));

            // Crew member mapping
            CreateMap<Crew, CrewDto>()
                .ForMember(
                    dest => dest.FullProfilePath,
                    opt => opt.MapFrom(src => GetFullImagePath(src.ProfilePath, ProfileSize)));
        }

        /// <summary>
        /// Configures mappings for genre entities
        /// </summary>
        private void ConfigureGenreMappings()
        {
            CreateMap<Genre, GenreDto>();
        }

        /// <summary>
        /// Constructs a full URL for an image path based on TMDB's image API
        /// </summary>
        /// <param name="path">Relative path to the image</param>
        /// <param name="size">Size specification for the image (e.g., "w500", "original")</param>
        /// <returns>Full URL to the image, or empty string if path is null/empty</returns>
        private static string GetFullImagePath(string path, string size)
        {
            // Return empty string for null or empty paths
            if (string.IsNullOrEmpty(path))
            {
                return string.Empty;
            }

            // If the path is already a full URL, return it as is
            if (path.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                return path;
            }

            // Ensure path starts with a slash
            if (!path.StartsWith("/", StringComparison.Ordinal))
            {
                path = "/" + path;
            }

            // Construct the full URL
            return $"{BaseImageUrl}{size}{path}";
        }
    }
}
