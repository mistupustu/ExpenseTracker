﻿using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace API.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [MinLength(5)]
        [MaxLength(50)]
        public required string Username { get; set; }

        [EmailAddress]
        public required string Email { get; set; }

        public required string Password { get; set; }

        public required UserRole Role { get; set; } = UserRole.USER;

        [JsonIgnore]
        public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
    }

}