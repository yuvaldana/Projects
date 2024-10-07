using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Interfaces
{
    public interface IPaymentRepository
    {
        Task<IEnumerable<PaymentModel>> GetPaymentAsync();
        Task<PaymentModel?> GetPaymentByPaymentIdAsync(int paymentId);
        Task<PaymentModel?> GetPaymentByOrderIdAsync(int orderId);
        Task UpdateToDBAsync(PaymentModel paymentUpdate);
        Task AddToDBAsync(PaymentModel payment);
    }
}
