namespace MovieCatalog.Domain.Models
{
    public class Crew
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Job { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string ProfilePath { get; set; } = string.Empty;
    }
}
