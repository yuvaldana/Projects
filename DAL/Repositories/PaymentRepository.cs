using Common.Interfaces;
using Common.Models;
using DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private PaymentDBContext _context;
        public PaymentRepository(PaymentDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PaymentModel>> GetPaymentAsync()
        {
            return await _context.Payments.ToListAsync();
        }
        public async Task<PaymentModel?> GetPaymentByPaymentIdAsync(int paymentId)
        {
            return await _context.Payments.FindAsync(paymentId);
        }
        public async Task<PaymentModel?> GetPaymentByOrderIdAsync(int orderId)
        {
            return await _context.Payments.FirstOrDefaultAsync(payment => payment.OrderId == orderId);
        }

        public async Task UpdateToDBAsync(PaymentModel paymentUpdate)
        {
            _context.Payments.Update(paymentUpdate);
            await _context.SaveChangesAsync();
        }
        public async Task AddToDBAsync(PaymentModel payment)
        {
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();
        }
    }
}
