using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace API.Models
{
    public class ExpenseCategory
    {
        [Key]
        public int Id { get; set; }
        [MinLength(3)]
        [MaxLength(50)]
        public required string Name { get; set; }
        [JsonIgnore]
        public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
    }
}
