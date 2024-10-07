using Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Interfaces
{
    public interface IUserService
    {
        Task AddAsync(AddUserModel userAdd);                        // Add a new user
        Task UpdateAsync(UserModel userUpdate);                     // Update an existing user
        Task<string> Authenticate(LoginModel userLogin);            // Authenticate a user
        Task DeleteAsync(UserModel userRemove);                     // Delete an existing user
    }
}
