using Microsoft.AspNetCore.Mvc;
using MovieAPI.Models;
using Microsoft.EntityFrameworkCore;
using MovieAPI.DAL;

namespace MovieAPI.Controllers;

[Route("[controller]")]
[ApiController]
public class MovieController : ControllerBase
{
    private readonly MovieApiContext _context;
    private readonly UnitOfWork unitOfWork;
    private readonly Dictionary<string, string> TableNames = new()
    {
        {"Ratings", nameof(Movie.Rating)},
        {"Genres", nameof(Movie.Genres)},
    };

    public MovieController(MovieApiContext context)
    {
        _context = context;
        unitOfWork = new(_context);
    }



    // Get All Movies

    [HttpGet]
    public async Task<ActionResult<Movie>> Get(int ratingId)
    {
        string tablesToInclude = $"{TableNames["Ratings"]},{TableNames["Genres"]}";

        ICollection<Movie> movies;

        if (ratingId > 0)
        {
            movies = await unitOfWork.MovieRepository.Get(includeProperties: tablesToInclude, filter: m => m.RatingId == ratingId);
        }
        else
        {
            movies = await unitOfWork.MovieRepository.Get(includeProperties: tablesToInclude);
        }

        return Ok(movies);
    }



    // Get Movie by Id

    [HttpGet("{movieId}")]
    public async Task<ActionResult<Movie>> GetById(int movieId)
    {
        string tablesToInclude = $"{TableNames["Ratings"]},{TableNames["Genres"]}";

        Movie? movie = await unitOfWork.MovieRepository.GetById(filter: m => m.Id == movieId, includeProperties: tablesToInclude);

        if (movie == null) return NotFound();


        return Ok(movie);
    }



    // Delete Movie

    [HttpDelete("{movieId}")]
    public async Task<IActionResult> Delete(int movieId)
    {
        Movie? movieToDelete = await unitOfWork.MovieRepository.GetById(filter: m => m.Id == movieId);

        if (movieToDelete == null)
        {
            return NotFound();
        }

        try
        {
            unitOfWork.MovieRepository.Delete(movieToDelete);
            await unitOfWork.Save();
            return Ok();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return BadRequest("Movie not deleted. Try again.");
        }
    }



    // Edit Movie

    [HttpPut("{movieId}")]
    public async Task<ActionResult<Movie>> Edit(int movieId, Movie newMovie)
    {
        if (movieId != newMovie.Id) return BadRequest();

        string tablesToInclude = $"{TableNames["Ratings"]},{TableNames["Genres"]}";

        Movie? movieToUpdate = await unitOfWork.MovieRepository.GetById(filter: m => m.Id == movieId, includeProperties: tablesToInclude);

        if (movieToUpdate == null) return NotFound();

        movieToUpdate.Title = newMovie.Title;
        movieToUpdate.Description = newMovie.Description;
        movieToUpdate.ReleaseDate = newMovie.ReleaseDate;
        movieToUpdate.RatingId = newMovie.RatingId;
        movieToUpdate.Rating = await unitOfWork.RatingRepository.GetById(r => r.Id == newMovie.RatingId);

        UpdateMovieGenres([.. newMovie.Genres], movieToUpdate);

        try
        {
            await unitOfWork.Save();
        }
        catch (DbUpdateException) when (!unitOfWork.MovieRepository.Exists(m => m.Id == movieId))
        {
            return NotFound();
        }
        return Ok(movieToUpdate);
    }

    private async void UpdateMovieGenres(List<Genre> selectedGenres, Movie movieToUpdate)
    {
        if (selectedGenres.Count <= 0)
        {
            movieToUpdate.Genres = [];
            return;
        }

        var selectedGenreIds = selectedGenres.Select(g => g.Id);

        var currentGenreIds = movieToUpdate.Genres.Select(g => g.Id);
        var genreIdsToAdd = selectedGenreIds.Except(currentGenreIds);
        var genreIdsToRemove = currentGenreIds.Except(selectedGenreIds);

        if (genreIdsToRemove.Any())
        {
            foreach (var genre in new List<Genre>(movieToUpdate.Genres.Where(g => genreIdsToRemove.Contains(g.Id))))
            {
                movieToUpdate.Genres.Remove(genre);
            }
        }

        if (genreIdsToAdd.Any())
        {
            var genresToAdd = await unitOfWork.GenreRepository.Get(filter: g => genreIdsToAdd.Contains(g.Id));
            foreach (var genre in genresToAdd)
            {
                movieToUpdate.Genres.Add(genre);
            }
        }
    }



    // Create Movie

    [HttpPost]
    public async Task<ActionResult<Movie>> Create([FromBody] Movie movieToAdd)
    {
        try
        {
            unitOfWork.MovieRepository.Insert(movieToAdd);
            await unitOfWork.Save();

            return CreatedAtAction(nameof(Get), new { movieToAdd.Id }, movieToAdd);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return BadRequest("Unable to save changes. Try again.");
        }
    }
}
