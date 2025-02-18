using API.Database;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/expenseCategory")]
    public class ExpenseCategoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ExpenseCategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExpenseCategory>>> GetAll()
        {
            return await _context.ExpenseCategories.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ExpenseCategory>> GetExpenseCategory(int id)
        {
            var expenseCategory = await _context.ExpenseCategories.
                FirstOrDefaultAsync(ec => ec.Id == id);

            if (expenseCategory == null)
                return NotFound("Unable to find expense category with specified id");

            return expenseCategory;
        }

        [HttpPost]
        public async Task<ActionResult<ExpenseCategory>> AddExpenseCateogry(ExpenseCategory category)
        {
            var validity = isCategoryValid(category);
            if (validity is BadRequestObjectResult)
            {
                return validity;
            }

            _context.ExpenseCategories.Add(category);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetExpenseCategory), new { id = category.Id }, category);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateExpenseCategory(int id, ExpenseCategory category)
        {
            var validity = isCategoryValid(category);
            if (validity is BadRequestObjectResult)
            {
                return validity;
            }
            if (id != category.Id)
                return BadRequest("Specified id does not match expense category id");

            _context.Entry(category).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(id))
                    return NotFound("Unable to find expense category with specified id");
                else
                    throw;
            }
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExpenseCategory(int id)
        {
            var category = await _context.ExpenseCategories.FindAsync(id);
            if (category == null)
                return NotFound("Unable to find expense category with specified id");

            _context.ExpenseCategories.Remove(category);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        [HttpGet("exists/{id}")]
        public bool CategoryExists(int id)
        {
            return _context.ExpenseCategories.Any(ec => ec.Id == id);
        }
        private ActionResult isCategoryValid(ExpenseCategory category)
        {
            if (String.IsNullOrEmpty(category.Name) || category.Name.Length < 3 || category.Name.Length > 50)
            {
                return BadRequest("expense category must be between 3 and 50 characters!");
            }

            if (_context.ExpenseCategories.Any(ec => ec.Name == category.Name))
                return BadRequest("an expense category with that name already exists");
            return Ok(new { message = "expense category is valid" });
        }
    }
}
