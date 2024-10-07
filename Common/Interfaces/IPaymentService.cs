using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentModel> CreateAsync(int orderId);
        Task<PaymentModel> UpdateAsync(int paymentID, int orderId);
        Task UpdateDetails(PaymentModel paymentDetailsUpdate);
        Task UpdateAmmount(int orderId, int ammount);
    }
}