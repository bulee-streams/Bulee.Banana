using System;
using System.Text;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using API.Models;
using API.Helpers;
using API.Repositories.Interfaces;

namespace API.Repositories
{
    public class UserRepository: IUserRepository
    {
        private readonly UserContext context;
        private readonly IConfiguration configuration;
        private readonly PasswordEncryption passwordEncrypt;

        public UserRepository(UserContext context, IConfiguration configuration, PasswordEncryption passwordEncrypt) {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this.passwordEncrypt = passwordEncrypt ?? throw new ArgumentNullException(nameof(passwordEncrypt));
        }

        public string Login(string username, string password)
        {
            var user = context.Users.Where(u => u.Username.ToUpper() == username.ToUpper() || 
                                                u.Email.ToUpper() == username.ToUpper()).FirstOrDefault();
            
            if(user == null) {
                return null;
            }

            if(passwordEncrypt.Hash(password, user.Salt) != user.Password) {
                return null;
            }

            var tokenHandler = new JwtSecurityTokenHandler();

            var something = configuration["Keys:JWTSecret"];
            var key = Encoding.ASCII.GetBytes(something);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
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
            if (Guid.TryParse(token, out Guid tokenGuid))
            {
                var user = context.Users.Where(u => u.EmailConfirmationToken == tokenGuid).FirstOrDefault();

                if (user != null)
                {
                    user.EmailConfirmed = true;
                    context.Update(user);
                    return await context.SaveChangesAsync() > 0 ? true : false;
                }

            }

            return false;
        }
    }
}
