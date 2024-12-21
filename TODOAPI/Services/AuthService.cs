using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TODOAPI.Data;
using TODOAPI.Models;

namespace TODOAPI.Services
{
    public class AuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;

        public AuthService(ApplicationDbContext context, IPasswordHasher<User> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        public async Task<User> RegisterUserAsync(string userName, string email, string password)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (existingUser != null)
            {
                throw new InvalidOperationException("Este e-mail já está registrado.");
            }

            var user = new User
            {
                UserName = userName,
                Email = email,
                PasswordHash = _passwordHasher.HashPassword(null, password)
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<User> AuthenticateUserAsync(string email, string password)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);

            if (user == null || _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password) != PasswordVerificationResult.Success)
            {
                return null; 
            }

            return user;
        }
    }
}
