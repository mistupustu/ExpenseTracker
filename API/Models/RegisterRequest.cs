using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class RegisterRequest
    {
        [MinLength(5)]
        [MaxLength(50)]
        public required string Username { get; set; }
        [MaxLength(50)]
        [MinLength(6)]
        public required string Password { get; set; }
        [EmailAddress]
        public required string Email { get; set; }
        public required UserRole Role { get; set; }
    }
}
