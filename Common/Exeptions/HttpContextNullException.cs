using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Exeptions
{
    public class HttpContextNullException : Exception
    {
        public HttpContextNullException() : base("HttpContext is null")
        {

        }
    }
}
