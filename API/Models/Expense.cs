using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    public class Expense
    {
        [Key]
        public int Id { get; set; }
        [Range(0, double.MaxValue)]
        public decimal Amount { get; set; }
        public ExpenseCategory? ExpenseCategory { get; set; }
        public int? ExpenseCategoryId { get; set; }
        public required User User { get; set; }
        public int UserId { get; set; }
    }
}
