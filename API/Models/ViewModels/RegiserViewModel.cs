using System.ComponentModel.DataAnnotations;

namespace API.Models.ViewModels
{
    public class RegiserViewModel
    {
        [Required(ErrorMessage = "Username can't be empty or null")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Email can't be empty or null")]
        [EmailAddress(ErrorMessage = "Email address must be formatted correctly")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
