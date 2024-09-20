using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MovieAPI.Models;

namespace MovieAPI.DAL;

public partial class MovieApiContext : IdentityDbContext<IdentityUser>
{
    public MovieApiContext(DbContextOptions<MovieApiContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Genre> Genres { get; set; }

    public virtual DbSet<Movie> Movies { get; set; }

    public virtual DbSet<Rating> Ratings { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("Name=ConnectionStrings:MovieAPIDatabase");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Genre>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("genres_pkey");

            entity.ToTable("genres");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.GenreType)
                .HasMaxLength(30)
                .HasColumnName("genretype");
        });

        modelBuilder.Entity<Movie>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("movies_pkey");

            entity.ToTable("movies");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.RatingId).HasColumnName("ratingid");
            entity.Property(e => e.ReleaseDate).HasColumnName("releasedate");
            entity.Property(e => e.Title).HasColumnName("title");

            entity.HasOne(d => d.Rating).WithMany(p => p.Movies)
                .HasForeignKey(d => d.RatingId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("movies_ratingid_fkey");

            entity.HasMany(d => d.Genres).WithMany(p => p.Movies)
                .UsingEntity<Dictionary<string, object>>(
                    "Moviegenre",
                    r => r.HasOne<Genre>().WithMany()
                        .HasForeignKey("Genreid")
                        .OnDelete(DeleteBehavior.Restrict)
                        .HasConstraintName("moviegenres_genreid_fkey"),
                    l => l.HasOne<Movie>().WithMany()
                        .HasForeignKey("Movieid")
                        .HasConstraintName("moviegenres_movieid_fkey"),
                    j =>
                    {
                        j.HasKey("Movieid", "Genreid").HasName("moviegenres_pkey");
                        j.ToTable("moviegenres");
                        j.IndexerProperty<int>("Movieid").HasColumnName("movieid");
                        j.IndexerProperty<int>("Genreid").HasColumnName("genreid");
                    });
        });

        modelBuilder.Entity<Rating>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("ratings_pkey");

            entity.ToTable("ratings");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.RatingType)
                .HasMaxLength(30)
                .HasColumnName("ratingtype");
            entity.Property(e => e.ShortRatingType)
                .HasMaxLength(5)
                .HasColumnName("shortratingtype");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
