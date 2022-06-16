using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using WebApiAuthors.DTOs;

namespace WebApiAuthors.Services
{
    public class HashService
    {
        public HashResult Hash(string text)
        {
            var salt = new byte[16];
            using (var random = RandomNumberGenerator.Create())
            {
                random.GetBytes(salt);
            }

            return Hash(text, salt);
        }

        public HashResult Hash(string text, byte[] salt)
        {
            var derivedKey = KeyDerivation.Pbkdf2(
                password: text,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA1,
                // recomendado autocompletado
                // prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 32);

            var hash = Convert.ToBase64String(derivedKey);

            return new HashResult
            {
                Hash = hash,
                Salt = salt
            };

            // forma mas directa
            /*return new HashResult()
            {
                Hash = Convert.ToBase64String(derivedKey),
                Salt = salt
            };*/
        }
    }
}
