using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieAPI.DAL;
using MovieAPI.Models;

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
    public async Task<ActionResult<Rating>> Get()
    {
        var ratings = await unitOfWork.RatingRepository.Get();

        return Ok(ratings);
    }

    [HttpGet("{ratingId}")]
    public async Task<ActionResult<Rating>> GetById(int ratingId)
    {
        Rating? rating = await unitOfWork.RatingRepository.GetById(filter: r => r.Id == ratingId);

        if (rating == null)
        {
            return NotFound();
        }

        return Ok(rating);
    }

    [HttpPost]
    public async Task<ActionResult<Rating>> Create([FromBody] Rating ratingToAdd)
    {
        try
        {
            unitOfWork.RatingRepository.Insert(ratingToAdd);
            await unitOfWork.Save();

            return CreatedAtAction(nameof(Get), new { ratingToAdd.Id }, ratingToAdd);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return BadRequest("Unable to save changes. Try again.");
        }
    }

    [HttpDelete("{ratingId}")]
    public async Task<IActionResult> Delete(int ratingId)
    {
        Rating? ratingToDelete = await unitOfWork.RatingRepository.GetById(filter: r => r.Id == ratingId);

        if (ratingToDelete == null)
        {
            return NotFound();
        }

        try
        {
            unitOfWork.RatingRepository.Delete(ratingToDelete);
            await unitOfWork.Save();
            return Ok();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return BadRequest("Rating not deleted. Try again.");
        }
    }

    [HttpPut("{ratingId}")]
    public async Task<ActionResult<Rating>> Edit(Rating newRating, int ratingId)
    {
        if (ratingId != newRating.Id) return BadRequest();

        Rating? originalRating = await unitOfWork.RatingRepository.GetById(r => r.Id == ratingId);

        if (originalRating == null) return NotFound();

        originalRating.ShortRatingType = newRating.ShortRatingType;
        originalRating.RatingType = newRating.RatingType;

        try
        {
            await unitOfWork.Save();
        }
        catch (DbUpdateException) when (!unitOfWork.RatingRepository.Exists(r => r.Id == ratingId))
        {
            return NotFound();
        }
        return Ok(originalRating);
    }
}
