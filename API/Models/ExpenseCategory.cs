using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class ExpenseCategory
    {
        [Key]
        public int Id { get; set; }
        [MinLength(5)]
        [MaxLength(50)]
        public required string Name { get; set; }
        public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
    }
}
