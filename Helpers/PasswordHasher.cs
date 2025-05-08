using Microsoft.AspNetCore.Identity;

namespace B2BUygulamasi.Helpers
{
    public static class PasswordHasher
    {
        private static readonly PasswordHasher<object> _hasher = new();

        public static string HashPassword(string password)
        {
            return _hasher.HashPassword(null, password);
        }

        public static bool VerifyPassword(string hashedPassword, string providedPassword)
        {
            var result = _hasher.VerifyHashedPassword(null, hashedPassword, providedPassword);
            return result == PasswordVerificationResult.Success;
        }
    }
}