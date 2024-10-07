using Common.Interfaces;
using Common.Models;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Org.BouncyCastle.Asn1.X509;
using Stripe;
using Stripe.Checkout;
using Stripe.FinancialConnections;
using Microsoft.Extensions.DependencyInjection;
using Common.Messages;
using Common.Exeptions;
using Microsoft.EntityFrameworkCore;

namespace PaymentService
{
    public class PaymentService : IPaymentService
    {
        private readonly IConfiguration _configuration;
        private readonly IPaymentRepository _paymentContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private IPublisher _publisher;

        public PaymentService(IConfiguration configuration, IPaymentRepository paymentRepository, IHttpContextAccessor httpContextAccessor, IPublisher publisher)
        {
            _configuration = configuration;
            _paymentContext = paymentRepository;
            _httpContextAccessor = httpContextAccessor;
            _publisher = publisher;
        }
        public async Task<PaymentModel> CreateAsync(int orderId)
        {
            try
            {
                var payment = await _paymentContext.GetPaymentByOrderIdAsync(orderId);
                if (payment == null)
                {
                    payment = new PaymentModel() { OrderId = orderId };
                }

                // Retrieve the Stripe secret key from IConfiguration
                StripeConfiguration.ApiKey = _configuration["StripeSettings:SecretKey"];
                if (_httpContextAccessor.HttpContext != null)
                {
                    // Payment config (currency, amount, etc.)
                    var options = new PaymentIntentCreateOptions
                    {
                        Amount = (long)(payment.TotalAmount * 100), // Amount should be in cents!
                        Currency = payment.Currency,
                        Description = payment.Description,
                        PaymentMethodTypes = new List<string> { "card" },
                        ReturnUrl = _httpContextAccessor.HttpContext.Request.Host + _httpContextAccessor.HttpContext.Request.Path + $"Checkout/Result"
                    };

                    // Make payment through stripe
                    // If payment is successful, update the order status to "Paid", Else keep Status as "Pending"
                    var service = new PaymentIntentService();
                    var paymentIntent = await service.CreateAsync(options);

                    if (paymentIntent.Status == "succeeded")
                    {
                        payment.PaymentStatus = PaymentStatus.Paid;
                        await _paymentContext.AddToDBAsync(payment);

                        await _publisher.Publish(new PaymentAttemptMessage(payment.OrderId, payment.PaymentStatus));
                    }
                    else
                    {
                        payment.PaymentStatus = PaymentStatus.Pending;
                        await _paymentContext.AddToDBAsync(payment);

                        await _publisher.Publish(new PaymentAttemptMessage(payment.OrderId, payment.PaymentStatus));
                    }
                    return payment;
                }
                else
                    throw new HttpContextNullException();
            }
            catch
            {
                throw new CreatePaymentFailException();
            }
        }

        public async Task<PaymentModel> UpdateAsync(int paymentID, int orderId)
        {
            try
            {
                PaymentModel? payment = new PaymentModel();
                if (paymentID != 0) // If paymentID is not 0 // paymentID != null since a value of type 'int' is never equal to 'null' of type 'int?'
                {
                    payment = await _paymentContext.GetPaymentByPaymentIdAsync(paymentID);
                }
                else if (orderId != 0) // If orderId is not 0 // orderId != null since a value of type 'int' is never equal to 'null' of type 'int?'
                {
                    payment = await _paymentContext.GetPaymentByOrderIdAsync(orderId);
                }
                else
                {
                    throw new UpdatePaymentException();
                }
                // Retrieve the Stripe secret key from IConfiguration
                StripeConfiguration.ApiKey = _configuration["StripeSettings:SecretKey"];

                if (_httpContextAccessor.HttpContext != null && payment != null)
                {
                    // Payment config (currency, amount, etc.)
                    var options = new PaymentIntentCreateOptions
                    {
                        Amount = (long)(payment.TotalAmount * 100), // Amount should be in cents!
                        Currency = payment.Currency,
                        Description = payment.Description,
                        PaymentMethodTypes = new List<string> { "card" },
                        ReturnUrl = _httpContextAccessor.HttpContext.Request.Host + _httpContextAccessor.HttpContext.Request.Path + $"Checkout/Result"
                    };

                    // Make payment through stripe
                    // If payment is successful, update the order status to "Paid", Else keep Status as "Pending"
                    var service = new PaymentIntentService();
                    var paymentIntent = await service.CreateAsync(options);
                    if (paymentIntent.Status == "succeeded")
                    {
                        payment.PaymentStatus = PaymentStatus.Paid;
                        await _paymentContext.UpdateToDBAsync(payment);

                        await _publisher.Publish(new PaymentAttemptMessage(payment.OrderId, payment.PaymentStatus));
                    }
                    else
                    {
                        payment.PaymentStatus = PaymentStatus.Pending;
                        await _paymentContext.UpdateToDBAsync(payment);

                        await _publisher.Publish(new PaymentAttemptMessage(payment.OrderId, payment.PaymentStatus));
                    }
                    return payment;
                }
                else
                    throw new HttpContextNullException();
            }
            catch
            {
                throw new UpdatePaymentException();
            }
        }
        public async Task UpdateDetails (PaymentModel paymentDetailsUpdate)
        {
            var existingDetails = await _paymentContext.GetPaymentByOrderIdAsync(paymentDetailsUpdate.PaymentId);
            if (existingDetails != null)
            {
                if (paymentDetailsUpdate.PaymentMethod != null)
                    existingDetails.PaymentMethod = paymentDetailsUpdate.PaymentMethod;
                if (paymentDetailsUpdate.Description != null)
                    existingDetails.Description = paymentDetailsUpdate.Description;
                existingDetails.TotalAmount = paymentDetailsUpdate.TotalAmount;  // Total amount cant be null
                existingDetails.PaymentStatus = paymentDetailsUpdate.PaymentStatus;  // Payment status cant be null
                if (paymentDetailsUpdate.ClientSecret != null)
                    existingDetails.ClientSecret = paymentDetailsUpdate.ClientSecret;
                if (paymentDetailsUpdate.Currency != null)
                    existingDetails.Currency = paymentDetailsUpdate.Currency;
                await _paymentContext.UpdateToDBAsync(existingDetails);
            }
        }
        public async Task UpdateAmmount(int orderId, int ammount)
        {
            try
            {
                PaymentModel? paymentUpdate = await _paymentContext.GetPaymentByOrderIdAsync(orderId);
                if (paymentUpdate != null)
                {
                    paymentUpdate.TotalAmount = ammount;
                    await UpdateDetails(paymentUpdate);
                }
            }
            catch
            {
                throw new SavingOrUpdatingDBException();
            }
        }
    }
}