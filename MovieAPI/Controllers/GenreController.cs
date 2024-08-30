using Microsoft.AspNetCore.Mvc;
using MovieAPI.Models;
using MovieAPI.DAL;
using Microsoft.EntityFrameworkCore;

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

    [HttpPost]
    public async Task<ActionResult<Genre>> Create([FromBody] Genre genreToAdd)
    {
        try
        {
            unitOfWork.GenreRepository.Insert(genreToAdd);
            await unitOfWork.Save();

            return CreatedAtAction(nameof(Get), new { genreToAdd.Id }, genreToAdd);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return BadRequest("Unable to save changes. Try again.");
        }
    }

    [HttpDelete("{genreId}")]
    public async Task<IActionResult> Delete(int genreId)
    {
        Genre? genreToDelete = await unitOfWork.GenreRepository.GetById(filter: g => g.Id == genreId);

        if (genreToDelete == null)
        {
            return NotFound();
        }

        try
        {
            unitOfWork.GenreRepository.Delete(genreToDelete);
            await unitOfWork.Save();
            return Ok();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return BadRequest("Genre not deleted. Try again.");
        }
    }

    [HttpPut("{genreId}")]
    public async Task<ActionResult<Genre>> Edit(Genre newGenre, int genreId)
    {
        if (genreId != newGenre.Id) return BadRequest();

        Genre? originalGenre = await unitOfWork.GenreRepository.GetById(g => g.Id == genreId);

        if (originalGenre == null) return NotFound();

        originalGenre.GenreType = newGenre.GenreType;

        try
        {
            await unitOfWork.Save();
        }
        catch (DbUpdateException) when (!unitOfWork.GenreRepository.Exists(g => g.Id == genreId))
        {
            return NotFound();
        }
        return Ok(originalGenre);
    }
}
