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

    [HttpGet]
    public async Task<ActionResult<Movie>> Get(int ratingId)
    {
        string tablesToInclude = $"{TableNames["Ratings"]},{TableNames["Genres"]}";

        ICollection<Movie> movies;

        if (ratingId > 0)
        {
            movies = await unitOfWork.MovieRepository.Get(includeProperties: tablesToInclude, filter: m => m.Ratingid == ratingId);
        }
        else
        {
            movies = await unitOfWork.MovieRepository.Get(includeProperties: tablesToInclude);
        }

        return Ok(movies);
    }

    [HttpGet("{movieId}")]
    public async Task<ActionResult<Movie>> GetById(int movieId)
    {
        string tablesToInclude = $"{TableNames["Ratings"]},{TableNames["Genres"]}";

        Movie? movie = await unitOfWork.MovieRepository.GetById(filter: m => m.Id == movieId, includeProperties: tablesToInclude);

        if (movie == null) return NotFound();


        return Ok(movie);
    }

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
}
