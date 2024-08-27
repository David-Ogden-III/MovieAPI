namespace Models;

public partial class Movie
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public int? Ratingid { get; set; }

    public DateOnly? Releasedate { get; set; }

    public virtual Rating? Rating { get; set; }

    public virtual ICollection<Genre> Genres { get; set; } = new List<Genre>();
}
