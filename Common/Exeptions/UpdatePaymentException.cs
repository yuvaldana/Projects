using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Exeptions
{
    public class UpdatePaymentException : Exception
    {
        public UpdatePaymentException() : base("Payment failed to get updated")
        {

        }
    }
}
