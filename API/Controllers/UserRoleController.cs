using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/userRoles")]
    public class UserRoleController : Controller
    {
        private readonly ApplicationDbContext _context;
        public UserRoleController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserRole>>> GetAll()
        {
            return await _context.UserRoles.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserRole>> GetUserRole(int id)
        {
            var userRole = await _context.UserRoles.
                FirstOrDefaultAsync(ur => ur.Id == id);

            if (userRole == null)
                return NotFound("Unable to find user role with specified id");

            return userRole;
        }

        [HttpPost]
        public async Task<ActionResult<UserRole>> AddUserRole(UserRole role)
        {
            var validity = isRoleValid(role);
            if (validity is BadRequestObjectResult)
            {
                return validity;
            }

            _context.UserRoles.Add(role);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUserRole), new {id = role.Id}, role);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUserRole(int id, UserRole role)
        {
            var validity = isRoleValid(role);
            if(validity is BadRequestObjectResult)
            {
                return validity;
            }
            if (id != role.Id)
                return BadRequest("Specified id does not match user role id");

            _context.Entry(role).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoleExists(id))
                    return NotFound("Unable to find user role with specified id");
                else
                    throw;
            }
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserRole(int id)
        {
            var role = await _context.UserRoles.FindAsync(id);
            if (role == null)
                return NotFound("Unable to find user role with specified id");

            _context.UserRoles.Remove(role);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        [HttpGet("exists/{id}")]
        public bool RoleExists(int id)
        {
            return _context.UserRoles.Any(ur => ur.Id == id);
        }
        private ActionResult isRoleValid(UserRole role)
        {
            if (String.IsNullOrEmpty(role.Name) || role.Name.Length < 3 || role.Name.Length > 30)
            {
                return BadRequest("User role must be between 3 and 30 characters!");
            }

            if (_context.UserRoles.Any(ur => ur.Name == role.Name))
                return BadRequest("A role with that name already exists");
            return Ok(new { message = "Role is valid" });
        }
    }
}
