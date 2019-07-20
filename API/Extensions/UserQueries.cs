using System.Linq;
using Microsoft.AspNetCore.Identity;
using API.Models;

namespace API.Extensions
{
    public class UserQueries : IUserQueries
    {
        public bool EmailExist(UserManager<User> manager, string email) =>
            manager.Users.Any(u => u.NormalizedEmail == email.ToUpper());

        public bool UserNameExist(UserManager<User> manager, string username) =>
            manager.Users.Any(u => u.NormalizedUserName == username.ToUpper());
    }
}
