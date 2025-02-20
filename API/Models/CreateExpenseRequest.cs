using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class CreateExpenseRequest
    {
        [Range(0, double.MaxValue)]
        public decimal Amount { get; set; }
        public int CategoryId { get; set; }
    }
}
