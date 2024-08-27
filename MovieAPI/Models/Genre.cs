using System.Text.Json.Serialization;

namespace MovieAPI.Models;

public partial class Genre
{
    public int Id { get; set; }

    public string? Genretype { get; set; }

    [JsonIgnore]
    public virtual ICollection<Movie> Movies { get; set; } = new List<Movie>();
}
