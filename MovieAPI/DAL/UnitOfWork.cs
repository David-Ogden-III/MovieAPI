using MovieAPI.Models;

namespace MovieAPI.DAL;

public class UnitOfWork : IDisposable
{
    private readonly MovieApiContext _context;
    private GenericRepository<Rating>? _ratingRepository;
    private GenericRepository<Genre>? _genreRepository;
    private GenericRepository<Movie>? _movieRepository;

    public UnitOfWork(MovieApiContext context)
    {
        _context = context;
    }

    public GenericRepository<Rating> RatingRepository
    {
        get
        {
            if (_ratingRepository == null)
            {
                _ratingRepository = new(_context);
            }
            return _ratingRepository;
        }
    }

    public GenericRepository<Genre> GenreRepository
    {
        get
        {
            if (_genreRepository == null)
            {
                _genreRepository = new(_context);
            }
            return _genreRepository;
        }
    }

    public GenericRepository<Movie> MovieRepository
    {
        get
        {
            if (_movieRepository == null)
            {
                _movieRepository = new(_context);
            }
            return _movieRepository;
        }
    }

    public void Save()
    {
        _context.SaveChanges();
    }




    private bool disposed = false;

    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                _context.Dispose();
            }
        }
        disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
