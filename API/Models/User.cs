using System;
using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Username { get; set; }

        public string Email { get; set; }

        public Guid EmailConfirmationToken { get; set; } = Guid.NewGuid();

        public bool EmailConfirmed { get; set; }

        public string Password { get; set; }

        public byte[] Salt { get; set; }

        public Guid PassworResetToken { get; set; }
    }
}
