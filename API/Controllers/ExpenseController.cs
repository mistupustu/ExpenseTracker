using API.Database;
using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace API.Controllers
{
    [ApiController]
    [Route("api/expense")]
    public class ExpenseController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ExpenseController> _logger;
        public ExpenseController(ApplicationDbContext context, ILogger<ExpenseController> logger)
        {
            _context = context;
            _logger = logger;
        }
        [HttpGet("all")]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult<IEnumerable<Expense>>> GetAll()
        {
            return await _context.Expenses.ToListAsync();
        }
        [HttpGet]
        [Authorize(Roles ="USER,ADMIN")]
        public async Task<ActionResult<IEnumerable<Expense>>> GetAllUsers()
        {
            var currentUsername = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userExpenses = await _context.Expenses
                .Where(e => e.User.Username == currentUsername)
                .ToListAsync();

            return userExpenses;
        }
        [HttpGet("all/{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult<Expense>> GetExpense(int id)
        {
            var expense = await _context.Expenses.
                FirstOrDefaultAsync(e => e.Id == id);

            if (expense == null)
                return NotFound("Expense with specified ID not found");

            return expense;
        }
        [HttpGet("{id}")]
        [Authorize(Roles ="ADMIN,USER")]
        public async Task<ActionResult<Expense>> GetUserExpense(int id)
        {
            var expense = await _context.Expenses.FirstOrDefaultAsync(e => e.Id == id);
            if (expense == null)
                return NotFound(new { message = "Unable to find expense with this id" });

            var currentUsername = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if (expense.User.Username != currentUsername)
                return Unauthorized(new { message = "You can only acces your own expenses" });
            return expense;
        }
        [HttpPost]
        [Authorize(Roles = "USER,ADMIN")]
        public async Task<ActionResult<Expense>> AddExpense(CreateExpenseRequest expenseRequest)
        {
            var expenseCategory = await _context.ExpenseCategories
                .FirstOrDefaultAsync(ec => ec.Id == expenseRequest.CategoryId);

            if (expenseCategory == null)
            {
                return BadRequest("Invalid category ID.");
            }

            var currentUsername = User.FindFirstValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name");
            _logger.LogInformation($"Current username: {currentUsername}");

            Expense expense = new()
            {
                Amount = expenseRequest.Amount,
                ExpenseCategoryId = expenseRequest.CategoryId,
                ExpenseCategory = expenseCategory,
                User = null
            };

            var validity = isValid(expense);
            if (validity is BadRequestObjectResult)
            {
                return validity;
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == currentUsername);

            if (user == null)
            {
                return BadRequest("User not found.");
            }

            expense.User = user;
            expense.UserId = user.Id;

            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetExpense), new { id = expense.Id }, expense);
        }


        [HttpPut("{id}")]
        [Authorize(Roles = "USER,ADMIN")]
        public async Task<IActionResult> UpdateExpense(int id, Expense expense)
        {
            var validity = isValid(expense);
            if (validity is BadRequestObjectResult)
            {
                return validity;
            }

            var existingExpense = await _context.Expenses.FindAsync(id);
            if (existingExpense == null)
                return NotFound("Unable to find expense with specified id");

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (existingExpense.User.Username != currentUserId && !User.IsInRole("ADMIN"))
                return Unauthorized("You can only update your own expenses.");

            _context.Entry(expense).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExpenseExists(id))
                    return NotFound("Unable to find expense with specified id");
                else
                    throw;
            }
            return NoContent();
        }
        [HttpDelete("{id}")]
        [Authorize(Roles = "USER,ADMIN")]
        public async Task<IActionResult> DeleteExpense(int id)
        {
            var expense = await _context.Expenses.FindAsync(id);
            if (expense == null)
                return NotFound("Unable to find expense with specified id");

            var currentUsername = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (expense.User.Username != currentUsername && !User.IsInRole("ADMIN"))
                return Unauthorized("You can only delete your own expenses.");

            _context.Expenses.Remove(expense);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        [HttpGet("exists/{id}")]
        [Authorize(Roles = "ADMIN")]
        public bool ExpenseExists(int id)
        {
            return _context.Expenses.Any(e => e.Id == id);
        }
        private ActionResult isValid(Expense expense)
        {
            if (expense.Amount <= 0)
                return BadRequest("Amount must be greater than 0");
            return Ok(new { message = "expense is valid" });
        }
    }
}
