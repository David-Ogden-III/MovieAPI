namespace MovieAPI.Models;

public partial class Movie
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public int? RatingId { get; set; }

    public DateOnly? ReleaseDate { get; set; }

    public virtual Rating? Rating { get; set; }

    public virtual ICollection<Genre> Genres { get; set; } = new List<Genre>();
}
