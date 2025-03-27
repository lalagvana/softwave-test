namespace MovieCatalog.Domain.Models
{
    public class PaginatedResult<T>
    {
        public int Page { get; set; }
        public int TotalPages { get; set; }
        public int TotalResults { get; set; }
        public IEnumerable<T> Results { get; set; } = new List<T>();
    }
}
