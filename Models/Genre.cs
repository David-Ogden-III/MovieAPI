namespace Models;

public partial class Genre
{
    public int Id { get; set; }

    public string? Genretype { get; set; }

    public virtual ICollection<Movie> Movies { get; set; } = new List<Movie>();
}
