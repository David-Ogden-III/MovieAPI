using Microsoft.AspNetCore.Mvc;
using MovieAPI.Models;
using Microsoft.EntityFrameworkCore;
using MovieAPI.DAL;

namespace MovieAPI.Controllers;

[Route("[controller]")]
[ApiController]
public class GenreController : ControllerBase
{
    private readonly MovieApiContext _context;

    public GenreController(MovieApiContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<Genre>> Get()
    {
        List<Genre> movies = await _context.Genres
            .ToListAsync();

        return Ok(movies);
    }

    [HttpGet("{genreId}")]
    public async Task<ActionResult<Genre>> GetById(int genreId)
    {
        Genre? selectedGenre = await _context.Genres.FindAsync(genreId);

        if (selectedGenre == null) return NotFound();

        return Ok(selectedGenre);
    }
}
