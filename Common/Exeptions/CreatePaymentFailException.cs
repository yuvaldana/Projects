using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Exeptions
{
    public class CreatePaymentFailException : Exception
    {
        public CreatePaymentFailException() : base("Payment failed to get created")
        {

        }
    }
}
