using Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.CodeDom.Compiler;
using Common.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.DependencyInjection;
using Common.Exeptions;

namespace API.Controllers
{
    [ApiController]
    public class ProductServiceController : Controller
    {
        private readonly IProductService _productManeger;

        public ProductServiceController(IProductService context)
        {
            _productManeger = context;
        }

        [AllowAnonymous]
        [Route("API/Product/Add")]
        [HttpPost]
        public IActionResult AddProduct([FromBody] ProductModel productToAdd)
        {
            try
            {
                _productManeger.AddAsync(productToAdd);
                if (productToAdd != null)
                {
                    return Ok(productToAdd);
                }
                return StatusCode(StatusCodes.Status400BadRequest, "No Product");
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [AllowAnonymous]
        [Route("API/Product/Update")]
        [HttpPost]
        public IActionResult UpdateProduct([FromBody] ProductModel productToUpdate)
        {
            try
            {
                _productManeger.UpdateAsync(productToUpdate);
                if (productToUpdate != null)
                {
                    return Ok(productToUpdate);
                }
                return StatusCode(StatusCodes.Status400BadRequest, "No Product");
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [AllowAnonymous]
        [Route("API/Product/Remove")]
        [HttpPost]
        public IActionResult DeleteProduct([FromBody] ProductModel productToRemove)
        {
            try
            {
                _productManeger.DeleteAsync(productToRemove);
                if (productToRemove != null)
                {
                    return Ok(productToRemove);
                }
                return StatusCode(StatusCodes.Status400BadRequest, "No Product");
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

    }
}
