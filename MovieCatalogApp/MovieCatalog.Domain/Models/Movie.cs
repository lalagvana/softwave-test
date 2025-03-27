namespace MovieCatalog.Domain.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? OriginalTitle { get; set; }
        public string Overview { get; set; } = string.Empty;
        public string PosterPath { get; set; } = string.Empty;
        public string BackdropPath { get; set; } = string.Empty;
        public decimal VoteAverage { get; set; }
        public int VoteCount { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public List<Genre> Genres { get; set; } = new();
        public List<Cast> Cast { get; set; } = new();
        public List<Crew> Crew { get; set; } = new();
        public string Status { get; set; } = string.Empty;
        public int? Runtime { get; set; }
        public decimal? Popularity { get; set; }
        public string? OriginalLanguage { get; set; }
        public bool Adult { get; set; }
        public bool Video { get; set; }
    }
}
