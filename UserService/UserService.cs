using Common.Models;
using DAL.Data;
using DAL.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System;
using Common.Interfaces;
using System.Security.Cryptography;
using System.IO;
using System.Text.RegularExpressions;
using System.Reflection.Metadata.Ecma335;
using Common.Exeptions;
using System.Threading.Tasks;

namespace UserService
{
    public class UserService : IUserService
    {
        private IConfiguration _configuration;
        private IUserRepository _context;
        public UserService(IConfiguration config, IUserRepository context)
        {
            _configuration = config;
            _context = context;
        }
        private string Generate(UserModel user)
        {
            // Create a new instance of the JwtSecurityToken class
            var jwtKey = _configuration["Jwt:DemoKey"] ?? throw new Exception("Jwt:DemoKey is missing in configuration.");
            byte[] byteJwtKey = Encoding.UTF8.GetBytes(jwtKey);
            var secureKey = new SymmetricSecurityKey(byteJwtKey);
            
            // Create a new instance of the SigningCredentials class
            var signingCredentials = new SigningCredentials((SymmetricSecurityKey)secureKey, SecurityAlgorithms.HmacSha256);

            // Create a new list of Claim objects
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserName),    // ClaimTypes.NameIdentifier is the user's username
                new Claim(ClaimTypes.Email, user.Email),                // ClaimTypes.Email is the user's email
                new Claim(ClaimTypes.Name, user.UserName),              // ClaimTypes.Name is the user's username
                new Claim(ClaimTypes.Role, user.Role)                   // ClaimTypes.Role is the user's role
            };

            // Create a new instance of the JwtSecurityToken class
            var token = new JwtSecurityToken(_configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,                                                 // The list of claims inside the token
                expires: DateTime.Now.AddMinutes(15),                   // The token will expire in 15 minutes
                signingCredentials: signingCredentials);                // The signing credentials

            return new JwtSecurityTokenHandler().WriteToken(token);     // Return the token
        }

        public async Task<string> Authenticate(LoginModel loginData)
        {
            try
            {
                // Get the user from the database
                UserModel currentUser = await _context.GetUserByEmailAsync(loginData.Email);

                // Check if the user exists and the password is correct
                if (currentUser != null)
                {
                    if (CompareArrays(currentUser.PasswordHash, await PasswordEncryptionAsync(loginData.Password)))
                    {
                        if (currentUser.Email.ToLower() == loginData.Email.ToLower())
                        {
                            // Generate a token for the user
                            var token = Generate(currentUser);
                            return token;  // Return the token
                        }
                        else
                        {
                            // If the user does not exist or the password is incorrect, throw an exception
                            throw new ValidateEmailPasswordException();
                        }
                    }
                    else
                    {
                        // If the user does not exist or the password is incorrect, throw an exception
                        throw new ValidateEmailPasswordException();
                    }
                }
                else
                {
                    // If the user does not exist or the password is incorrect, throw an exception
                    throw new ValidateEmailPasswordException();
                }
            }

            catch (Exception e)
            {
                // If an exception occurs, throw an exception
                throw new AuthenticateFailException(e);
            }
        }
        private bool CompareArrays(byte[] current, byte[] compare) // Compare two byte arrays
        {
            // Check if the arrays have different lengths
            if (current.Length != compare.Length)
            {
                return false;
            }
            // Compare each byte in the arrays
            for (int i = 0; i < current.Length; i++)
            {
                if (current[i] != compare[i])
                {
                    return false;
                }
            }
            // If all checks pass, the arrays are equal
            return true;
        }

        public async Task AddAsync(AddUserModel userAdd) // Add a new user to the database
        {
            try
            {
                // Check if the password is strong enough
                if (!(await PasswordStrenghtAsync(userAdd.Password)))
                {
                    // If the password is not strong enough, throw an exception
                    throw new PasswordFailExeption();
                }
                // Check if the email is valid
                if (!(await EmailValidationAsync(userAdd.Email)))
                {
                    // If the email is not valid, throw an exception
                    throw new EmailFailExeption();
                }
                // Create a new instance of the UserModel class
                var user = new UserModel();
                // -------------------------------------------------------------------- // Set the properties of the user
                user.ID = userAdd.ID;                                                   // The ID of the user
                user.UserName = userAdd.UserName;                                       // The username of the user
                user.Email = userAdd.Email;                                             // The email of the user
                user.PasswordHash = await PasswordEncryptionAsync(userAdd.Password);    // The password of the user
                user.CreatedTime = DateTime.Now;                                        // The time the user was created
                user.UpdatedTime = DateTime.Now;                                        // The time the user was last updated
                user.Role = userAdd.Role;                                               // The role of the user
                user.FirstName = userAdd.FirstName;                                     // The first name of the user
                user.LastName = userAdd.LastName;                                       // The last name of the user
                user.BirthDay = userAdd.BirthDay;                                       // The birthday of the user
                user.Address = userAdd.Address;                                         // The address of the user
                user.City = userAdd.City;                                               // The city of the user
                user.Country = userAdd.Country;                                         // The country of the user
                user.ZipCode = userAdd.ZipCode;                                         // The zip code of the user
                user.Phone = userAdd.Phone;                                             // The phone number of the user

                // Add the user to the database
                await _context.AddToDBAsync(user);
            }
            catch
            {
                // If an exception occurs, throw an exception
                throw new SavingOrUpdatingDBException();
            }
        }

        public async Task<byte[]> PasswordEncryptionAsync(string password) // Encrypt the password
        {
            using (SHA256 sha256 = SHA256.Create()) // Create a new instance of the SHA256 class
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);              // Convert the password to a byte array
                return await Task.Run(() => sha256.ComputeHash(passwordBytes));       // Return the encrypted password
            }
        }

        public Task<bool> PasswordStrenghtAsync(string password) // Check if the password is strong enough
        {
            // Create a new task
            return Task.Run(() =>
            {
                bool containsUppercase = false;                                     
                bool containsLowercase = false;
                bool containsDigit = false;
                bool containsSpecialCharacter = false;
                string specialCharacters = "!@#$%^&*()-_=+[]{}\\|;:'\",.<>/?";

                foreach (char c in password)
                {
                    if (Char.IsUpper(c))                        // Check if the character is uppercase
                    {
                        containsUppercase = true;
                    }
                    else if (Char.IsLower(c))                   // Check if the character is lowercase
                    {
                        containsLowercase = true;
                    }
                    else if (Char.IsDigit(c))                   // Check if the character is digit
                    {
                        containsDigit = true;
                    }
                    else if (specialCharacters.Contains(c))     // Check if the character is a special character
                    {
                        containsSpecialCharacter = true;
                    }
                }

                // Return true if the password is strong enough
                return containsUppercase && containsLowercase && containsDigit && containsSpecialCharacter && password.Length >= 8; 
            });
        }

        public Task<bool> EmailValidationAsync(string email) // Check if the email is valid
        {
            // Create a new task
            return Task.Run(() =>
            {
                string pattern = "^[a-zA-Z0-9.!#$%&’*+/=?^_`{|}~-]+@[a-zA-Z0-9-]+(?:\\.[a-zA-Z0-9-]+)*$";   // The pattern for a valid email
                bool isValid = false;
                isValid = Regex.IsMatch(email, pattern);   // Check if the email matches the pattern
                return isValid;    // Return the result  
            });
        }
        public async Task UpdateAsync(UserModel userUpdate) // Update the user
        {
            var existingUser = await _context.GetUserByIDAsync(userUpdate.ID); // Get the user from the database

            if (existingUser != null) // Check if the user exists
            {
                try
                {
                    existingUser.UpdatedTime = DateTime.Now;                           // Set the updated time to the current time

                    // Update only the properties that are not null in userUpdate - Patch request data
                    if (userUpdate.Email != null)                                 // Patch Email     // This should be also validated, but for the sake of the example
                        existingUser.Email = userUpdate.Email;

                    if (userUpdate.PasswordHash != null)                          // Patch Password  // This should be string password then validate it and encrypt it, but for the sake of the example
                        existingUser.PasswordHash = userUpdate.PasswordHash;

                    if (userUpdate.Role != null)                                  // Patch Role
                        existingUser.Role = userUpdate.Role;

                    if (userUpdate.FirstName != null)                             // Patch FirstName
                        existingUser.FirstName = userUpdate.FirstName;

                    if (userUpdate.LastName != null)                              // Patch LastName
                        existingUser.LastName = userUpdate.LastName;

                    if (userUpdate.BirthDay != null)                              // Patch BirthDay
                        existingUser.BirthDay = userUpdate.BirthDay;

                    if (userUpdate.Address != null)                               // Patch Address
                        existingUser.Address = userUpdate.Address;

                    if (userUpdate.City != null)                                  // Patch City
                        existingUser.City = userUpdate.City;

                    if (userUpdate.Country != null)                               // Patch Country
                        existingUser.Country = userUpdate.Country;

                    if (userUpdate.ZipCode != null)                               // Patch ZipCode
                        existingUser.ZipCode = userUpdate.ZipCode;

                    if (userUpdate.Phone != null)                                 // Patch Phone
                        existingUser.Phone = userUpdate.Phone;

                    // Update the user in the database
                    await _context.UpdateToDBAsync(existingUser);
                }
                catch
                {
                    // If an exception occurs, throw an exception
                    throw new SavingOrUpdatingDBException();
                }
            }
        }
        public async Task DeleteAsync(UserModel userRemove) // Delete the user
        {
            try
            {
                await _context.RemoveFromDBAsync(userRemove.ID); // Remove the user from the database
            }
            catch
            {
                // If an exception occurs, throw an exception
                throw new SavingOrUpdatingDBException();
            }
        }
    }
}