using Microsoft.AspNetCore.Mvc;
using MovieAPI.Models;
using MovieAPI.DAL;

namespace MovieAPI.Controllers;

[Route("[controller]")]
[ApiController]
public class GenreController : ControllerBase
{
    private readonly MovieApiContext _context;
    private readonly UnitOfWork unitOfWork;

    public GenreController(MovieApiContext context)
    {
        _context = context;
        unitOfWork = new(_context);
    }

    [HttpGet]
    public async Task<ActionResult<Genre>> Get()
    {
        var genres = await unitOfWork.GenreRepository.Get();

        return Ok(genres);
    }

    [HttpGet("{genreId}")]
    public async Task<ActionResult<Genre>> GetById(int genreId)
    {
        Genre? selectedGenre = await unitOfWork.GenreRepository.GetById(filter: g => g.Id == genreId);

        if (selectedGenre == null) return NotFound();

        return Ok(selectedGenre);
    }
}
