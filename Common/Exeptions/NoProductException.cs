using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Exeptions
{
    public class NoProductException : Exception
    {
        public NoProductException() : base("No Product")
        {

        }
    }
}
