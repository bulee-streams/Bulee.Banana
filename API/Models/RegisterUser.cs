using System;

namespace API.Models
{
    public class RegisterUser
    {
        public Guid Id { get; set; }
        public DateTime TimeAdded { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
    }
}
