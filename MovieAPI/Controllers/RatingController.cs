using Microsoft.AspNetCore.Mvc;
using MovieAPI.Models;
using Microsoft.EntityFrameworkCore;
using MovieAPI.DAL;

namespace MovieAPI.Controllers;

[Route("[controller]")]
[ApiController]
public class RatingController : ControllerBase
{
    private readonly MovieApiContext _context;

    public RatingController(MovieApiContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Rating>>> Get()
    {
        List<Rating> rating = await _context.Ratings.ToListAsync();

        return Ok(rating);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<IEnumerable<Rating>>> GetById(int id)
    {
        Rating? rating = await _context.Ratings.FindAsync(id);

        if (rating == null)
        {
            return NotFound();
        }

        return Ok(rating);
    }
}
