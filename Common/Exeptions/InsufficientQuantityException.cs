using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Exeptions
{
    public class InsufficientQuantityException : Exception
    {
        public InsufficientQuantityException() : base("Not enough products in stock")
        {
        }
    }
}
