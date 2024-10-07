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
    public class PaymentServiceController : Controller
    {
        private readonly IPaymentService _paymentManeger;

        public PaymentServiceController(IPaymentService paymentMeneger)
        {
            _paymentManeger = paymentMeneger;
        }

        [AllowAnonymous]
        [Route("API/Payment/Create")]
        [HttpPost]
        public IActionResult Create([FromBody] PaymentModel paymentToAdd)
        {
            try
            {
                _paymentManeger.CreateAsync(paymentToAdd.OrderId);
                if (paymentToAdd != null)
                {
                    return Ok(paymentToAdd);
                }
                return StatusCode(StatusCodes.Status400BadRequest, "No Payment");
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }
        [AllowAnonymous]
        [Route("API/Payment/Update")]
        [HttpPost]
        public IActionResult Update([FromBody] PaymentModel paymentToUpdate)
        {
            try
            {
                _paymentManeger.UpdateAsync(paymentToUpdate.PaymentId, paymentToUpdate.OrderId);
                if (paymentToUpdate != null)
                {
                    return Ok(paymentToUpdate);
                }
                return StatusCode(StatusCodes.Status400BadRequest, "No Payment");
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }
    }
}
