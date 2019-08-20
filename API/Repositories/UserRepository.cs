using System;
using System.Linq;
using System.Threading.Tasks;
using API.Context;
using API.Models;
using API.Repositories.Interfaces;

namespace API.Repositories
{
    public class UserRepository: IUserRepository
    {
        private readonly UserContext context;
        private readonly PasswordEncryption passwordEncrypt;

        public UserRepository(UserContext context, PasswordEncryption passwordEncrypt) {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
            this.passwordEncrypt = passwordEncrypt ?? throw new ArgumentNullException(nameof(passwordEncrypt));
        }

        public async Task<User> Create(string username, string email, string password)
        {
            var encryptedPassword = passwordEncrypt.HashReturnSalt(password);
            var user = new User() { Username = username, Email = email, Salt = encryptedPassword.Item1, Password = encryptedPassword.Item2 };

            await context.AddAsync(user);
            return await context.SaveChangesAsync() > 0 ? user : null; 
        }

        public bool DoesUsernameExist(string username) =>
            context.Users.Any(u => u.Username.ToUpper() == username.ToUpper());

        public bool DoesEmailExist(string email) =>
            context.Users.Any(u => u.Email.ToUpper() == email.ToUpper());

        public async Task<bool> IsEmailConfirmationValid(string token)
        {
            Guid tokenGuid;
            if (Guid.TryParse(token, out tokenGuid)) {
                var user = context.Users.Where(u => u.EmailConfirmationToken == tokenGuid).FirstOrDefault();

                if(user != null) {
                    user.EmailConfirmed = true;
                    context.Update(user);
                   return await context.SaveChangesAsync() > 0 ? true : false;
                }

            }

            return false;
        }
    }
}
