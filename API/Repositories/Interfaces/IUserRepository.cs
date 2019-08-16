using System.Threading.Tasks;
using API.Models;

namespace API.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User> Create(string username, string email, string password);
        bool DoesUsernameExist(string username);
        bool DoesEmailExist(string email);
        Task<bool> IsEmailConfirmationValid(string token);
    }
}
