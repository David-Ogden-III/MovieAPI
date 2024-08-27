using Microsoft.AspNetCore.Mvc;
using MovieAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace MovieAPI.Controllers;

[Route("[controller]")]
[ApiController]
public class MovieController : ControllerBase
{
    private readonly MovieApiContext _context;

    public MovieController(MovieApiContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<Movie>> Get()
    {
        List<Movie> movies = await _context.Movies
            .Include(m => m.Rating)
            .Include(m => m.Genres)
            .ToListAsync();

        return Ok(movies);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Movie>> GetById(int id)
    {
        Movie? movie = await _context.Movies
            .Include(m => m.Rating)
            .Include(m => m.Genres)
            .FirstOrDefaultAsync(movie => movie.Id == id);

        if (movie == null) return NotFound();
        

        return Ok(movie);
    }
}
