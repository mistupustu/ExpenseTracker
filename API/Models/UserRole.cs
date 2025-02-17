using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class UserRole
    {
        [Key]
        public int Id { get; set; }
        [MinLength(3)]
        [MaxLength(30)]
        public required string Name { get; set; }
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
