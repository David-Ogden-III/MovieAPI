using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MovieAPI.DAL;

namespace MovieAPI.Controllers;

[Route("[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]
public class RoleController : ControllerBase
{
    private readonly MovieApiContext _context;
    private readonly UnitOfWork unitOfWork;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<IdentityUser> _userManager;

    public RoleController(MovieApiContext context, RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
    {
        _context = context;
        unitOfWork = new(_context);
        _roleManager = roleManager;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<ActionResult<IdentityRole>> Get()
    {
        var roles = await unitOfWork.RoleRepository.Get();

        return Ok(roles);
    }

    [HttpPost("CreateRole")]
    public async Task<ActionResult<IdentityRole>> Create([FromBody] string roleName)
    {
        try
        {
            IdentityRole roleToAdd = new(roleName)
            {
                ConcurrencyStamp = Guid.NewGuid().ToString()
            };
            bool success = (await _roleManager.CreateAsync(roleToAdd)).Succeeded;

            if (success)
            {
                return CreatedAtAction(nameof(Get), new { roleToAdd.Id }, roleToAdd);
            }
            else
            {
                return BadRequest();
            }


        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return BadRequest("Unable to save changes. Try again.");
        }
    }

    [HttpPost("AddRoleToUser/{email}")]
    public async Task<ActionResult<IdentityRole>> AddRoleToUser([FromBody] string roleName, string email)
    {
        try
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            var user = await _userManager.FindByEmailAsync(email);

            if (role == null || user == null) return BadRequest();

            bool success = (await _userManager.AddToRoleAsync(user, role.Name)).Succeeded;

            if (success)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }


        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return BadRequest("Unable to save changes. Try again.");
        }
    }
}
