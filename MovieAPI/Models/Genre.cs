using System.Text.Json.Serialization;

namespace MovieAPI.Models;

public partial class Genre
{
    public int Id { get; set; }

    public string? GenreType { get; set; }
    public string? UserId { get; set; }

    [JsonIgnore]
    public virtual ICollection<Movie> Movies { get; set; } = new List<Movie>();

    public virtual User? User { get; set; }
}
