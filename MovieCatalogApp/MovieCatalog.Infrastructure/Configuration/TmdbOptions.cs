namespace MovieCatalog.Infrastructure.Configuration
{
    public class TmdbOptions
    {
        public string ApiKey { get; set; } = string.Empty;
        public string BaseUrl { get; set; } = string.Empty;
        public string ImageBaseUrl { get; set; } = string.Empty;
        public int CacheExpirationMinutes { get; set; } = 15;
    }
}
