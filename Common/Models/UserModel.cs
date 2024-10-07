using Foolproof;
using Microsoft.AspNetCore.Identity;
using System;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class UserModel
    {
        [Required]
        public int ID { get; set; }
        public string? UserName { get; set; }

        [Required]
        public string Email { get; set; } = null!;

        [Required]
        public byte[] PasswordHash { get; set; } = null!;
        public DateTime CreatedTime { get; set; }
        public DateTime UpdatedTime { get; set;}
        public string? Role { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateOnly? BirthDay { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? ZipCode { get; set; }
        public string? Phone { get; set; }
    }
}