using System.Text.Json.Serialization;

namespace MovieAPI.Models;

public partial class Rating
{
    public int Id { get; set; }

    public string? Shortratingtype { get; set; }

    public string? Ratingtype { get; set; }

    [JsonIgnore]
    public virtual ICollection<Movie> Movies { get; set; } = new List<Movie>();
}
