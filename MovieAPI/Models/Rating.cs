namespace MovieAPI.Models;

public partial class Rating
{
    public int Id { get; set; }

    public string? Shortratingtype { get; set; }

    public string? Ratingtype { get; set; }

    public virtual ICollection<Movie> Movies { get; set; } = new List<Movie>();
}
