using System;

namespace API.Models.ViewModels
{
    public class PasswordResetViewModel
    {
        public Guid Token { get; set; }

        public string Password { get; set; }
    }
}
