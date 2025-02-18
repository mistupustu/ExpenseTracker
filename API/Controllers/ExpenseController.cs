using API.Database;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/expense")]
    public class ExpenseController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ExpenseController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Expense>>> GetAll()
        {
            return await _context.Expenses.ToListAsync();
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Expense>> GetExpense(int id)
        {
            var expense = await _context.Expenses.
                FirstOrDefaultAsync(e => e.Id == id);

            if (expense == null)
                return NotFound("Expense with specified ID not found");

            return expense;
        }
        [HttpPost]
        public async Task<ActionResult<Expense>> AddExpense(Expense expense)
        {
            var validity = isValid(expense);
            if (validity is BadRequestObjectResult)
            {
                return validity;
            }

            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetExpense), new {id = expense.Id}, expense);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateExpense(int id, Expense expense)
        {
            var validity = isValid(expense);
            if (validity is BadRequestObjectResult)
            {
                return validity;
            }
            if (id != expense.Id)
                return BadRequest("Specified id does not match expense id");

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
        public async Task<IActionResult> DeleteExpense(int id)
        {
            var expense = await _context.Expenses.FindAsync(id);
            if (expense == null)
                return NotFound("Unable to find expense with specified id");

            _context.Expenses.Remove(expense);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        [HttpGet("exists/{id}")]
        public bool ExpenseExists(int id)
        {
            return _context.Expenses.Any(e => e.Id == id);
        }
        private ActionResult isValid(Expense expense)
        {
            if (expense.Amount <= =)
                return BadRequest("Amount must be greater than 0");
            return Ok(new { message = "expense is valid" });
        }
    }
}
