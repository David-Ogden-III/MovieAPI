using Microsoft.AspNetCore.Mvc;
using MovieAPI.Models;
using MovieAPI.DAL;

namespace MovieAPI.Controllers;

[Route("[controller]")]
[ApiController]
public class RatingController : ControllerBase
{
    private readonly MovieApiContext _context;
    private readonly UnitOfWork unitOfWork;

    public RatingController(MovieApiContext context)
    {
        _context = context;
        unitOfWork = new(_context);
    }


    [HttpGet]
    public async Task<ActionResult<IEnumerable<Rating>>> Get()
    {
        var ratings = await unitOfWork.RatingRepository.Get();

        return Ok(ratings);
    }

    [HttpGet("{ratingId}")]
    public async Task<ActionResult<IEnumerable<Rating>>> GetById(int ratingId)
    {
        Rating? rating = await unitOfWork.RatingRepository.GetById(filter: r => r.Id == ratingId);

        if (rating == null)
        {
            return NotFound();
        }

        return Ok(rating);
    }
}
