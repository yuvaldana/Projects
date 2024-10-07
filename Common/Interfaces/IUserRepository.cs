using Common.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<UserModel>> GetUserAsync();                // Get all users
        Task<UserModel?> GetUserByEmailAsync(string email);          // Get user by email
        Task AddToDBAsync(UserModel user);                          // Add user to database
        Task UpdateToDBAsync(UserModel userUpdate);                 // Update user in database
        Task RemoveFromDBAsync(int userId);                         // Remove user from database
        Task<UserModel?> GetUserByIDAsync(int userId);               // Get user by ID
    }
}
