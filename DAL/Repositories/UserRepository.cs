using Common.Interfaces;
using Common.Models;
using DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
        public class UserRepository : IUserRepository
    {
        private UserDBContext _context;

        public UserRepository(UserDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserModel>> GetUserAsync() // Get all users
        {
            return await _context.Users.ToListAsync(); // Return all users
        }

        public async Task<UserModel?> GetUserByEmailAsync(string email) // Get user by email
        {
            return await _context.Users.FirstOrDefaultAsync(i => i.Email == email); // Return user by email
        }

        public async Task AddToDBAsync(UserModel user) // Add user to database
        {
            _context.Users.Add(user); // Add user to database
            await _context.SaveChangesAsync(); // Save changes
        }

        public async Task UpdateToDBAsync(UserModel userUpdate) // Update user in database
        {
            _context.Users.Update(userUpdate); // Update user in database
            await _context.SaveChangesAsync(); // Save changes
        }

        public async Task RemoveFromDBAsync(int userId) // Remove user from database
        {
            var userToRemove = await GetUserByIDAsync(userId); // Get user by ID

            if (userToRemove != null) // If user exists
            {
                _context.Users.Remove(userToRemove); // Remove user
                await _context.SaveChangesAsync(); // Save changes
            }
        }

        public async Task<UserModel?> GetUserByIDAsync(int userId) // Get user by ID
        {
            return await _context.Users.FindAsync(userId); // Return user by ID
        }
    }
}