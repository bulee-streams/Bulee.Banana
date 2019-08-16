using System;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace API
{
    public class PasswordEncryption
    {
        private readonly byte[] salt;
        private readonly RandomNumberGenerator rng;

        public PasswordEncryption()
        {
            salt = new byte[128 / 8];
            rng = RandomNumberGenerator.Create();
        }

        public string Hash(string password)
        {
            rng.GetBytes(salt);
            return Hash(password, salt);
        }

        public (byte[], string) HashReturnSalt(string password) =>
                (salt, Hash(password));

        public string Hash(string password, byte[] salt)
        {
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));
        }
    }
}
