namespace MovieCatalog.Domain.Models
{
    public class Cast
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Character { get; set; } = string.Empty;
        public string ProfilePath { get; set; } = string.Empty;
        public int Order { get; set; }
    }
}
