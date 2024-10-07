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
    public class OrderServiceController : Controller
    {
        private readonly IOrderService _orderManeger;

        public OrderServiceController(IOrderService orderMeneger)
        {
            _orderManeger = orderMeneger;
        }

        [AllowAnonymous]
        [Route("API/Order/Add")]
        [HttpPost]
        public IActionResult AddOrder([FromBody] OrderModel orderToAdd)
        {
            try
            {
                _orderManeger.AddAsync(orderToAdd);
                if (orderToAdd != null)
                {
                    return Ok(orderToAdd);
                }
                return StatusCode(StatusCodes.Status400BadRequest, "No Order");
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }
        [AllowAnonymous]
        [Route("API/Order/Update")]
        [HttpPost]
        public IActionResult UpdateOrder([FromBody] OrderModel orderToUpdate)
        {
            try
            {
                _orderManeger.UpdateAsync(orderToUpdate);
                if (orderToUpdate != null)
                {
                    return Ok(orderToUpdate);
                }
                return StatusCode(StatusCodes.Status400BadRequest, "No Order");
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [AllowAnonymous]
        [Route("API/Order/Remove")]
        [HttpPost]
        public IActionResult DeleteOrder([FromBody] OrderModel orderRemove)
        {
            try
            {
                _orderManeger.DeleteAsync(orderRemove);
                if (orderRemove != null)
                {
                    return Ok(orderRemove);
                }
                return StatusCode(StatusCodes.Status400BadRequest, "No Order");
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

    }
}
