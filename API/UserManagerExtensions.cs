using System.Linq;
using Microsoft.AspNetCore.Identity;
using API.Models;

namespace API
{
    public static class UserManagerExtensions
    {
        public static bool UserNameExists(this UserManager<User> manager, string name) =>
            manager.Users.Any(u => u.NormalizedUserName == name.ToUpper());

        public static bool EmailExists(this UserManager<User> manager, string email) =>
            manager.Users.Any(u => u.NormalizedEmail == email.ToUpper());
    }
}
