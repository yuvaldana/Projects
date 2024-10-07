using Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.CodeDom.Compiler;
using Common.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.DependencyInjection;
using Common.Exeptions;

namespace API.Controllers
{
    [ApiController]
    public class UserServiceController : Controller
    {
        private readonly IUserService _userMeneger;
        private readonly IUserService _context;
        
        public UserServiceController(IUserService userMeneger, IUserService context)
        {
            _userMeneger = userMeneger;
            _context = context;
        }

        [AllowAnonymous]
        [Route("API/User/Login")]
        [HttpPost]      
        public IActionResult Login([FromBody] LoginModel userLogin)
        {
            try
            {
                var token = _userMeneger.Authenticate(userLogin);
                if (token != null)
                {
                    return Ok(token);
                }
                return Unauthorized("No User");
            }
            catch(Exception e)
            { 
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }
        
        [AllowAnonymous]
        [Route("API/User/Register")]
        [HttpPost]
        public IActionResult AddUser([FromBody] AddUserModel userAdd)
        {
            try
            {
                _context.AddAsync(userAdd);
                return Ok();
            }
            catch(PasswordFailExeption e)
            {
                return BadRequest(e.Message);
            }
            catch(EmailFailExeption e)
            {
                return BadRequest(e.Message);
            }
            catch(Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }
        [AllowAnonymous]
        [Route("API/User/Update")]
        [HttpPost]
        public IActionResult UpdateUser([FromBody] UserModel userToUpdate)
        {
            try
            {
                _context.UpdateAsync(userToUpdate);
                if (userToUpdate != null)
                {
                    return Ok(userToUpdate);
                }
                return StatusCode(StatusCodes.Status400BadRequest, "No User");
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [AllowAnonymous]
        [Route("API/User/Remove")]
        [HttpPost]
        public IActionResult DeleteUser([FromBody] UserModel userToRemove)
        {
            try
            {
                _context.DeleteAsync(userToRemove);
                if (userToRemove != null)
                {
                    return Ok(userToRemove);
                }
                return StatusCode(StatusCodes.Status400BadRequest, "No User");
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

    }
}
