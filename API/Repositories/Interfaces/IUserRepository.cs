using System;
using System.Threading.Tasks;
using API.Models;

namespace API.Repositories.Interfaces
{
    public interface IUserRepository
    {
        string Login(string username, string password);

        Task<User> Create(string username, string email, string password);
        bool DoesUsernameExist(string username);
        bool DoesEmailExist(string email);
        Task<bool> IsEmailConfirmationValid(string token);

        Task<Guid> CreatePasswordResetToken(string username);

        Task<bool> PasswordReset(Guid token, string password);
    }
}
