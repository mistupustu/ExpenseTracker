using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace API.Models
{
    public class Expense
    {
        [Key]
        public int Id { get; set; }
        [Range(0, double.MaxValue)]
        public decimal Amount { get; set; }
        [JsonIgnore]
        public ExpenseCategory? ExpenseCategory { get; set; }
        public int? ExpenseCategoryId { get; set; }
        [JsonIgnore]
        public User User { get; set; }
        public int UserId { get; set; }
    }
}
