using Microsoft.AspNetCore.Identity;
using API.Models;
namespace API.Extensions
{
    public interface IUserQueries
    {
        bool UserNameExist(UserManager<User> manager, string username);

        bool EmailExist(UserManager<User> manager, string email);
    }
}
