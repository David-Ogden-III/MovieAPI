using Microsoft.AspNetCore.Mvc;
using MovieAPI.Models;
using Microsoft.Extensions.Logging;

namespace MovieAPI.Controllers;

[Route("[controller]")]
[ApiController]
public class RatingController : ControllerBase
{
    private readonly ILogger<RatingController> _logger;
    private readonly MovieApiContext _context;

    public RatingController(ILogger<RatingController> logger, MovieApiContext context)
    {
        _logger = logger;
        _context = context;
    }
}
