using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Exeptions
{
    public class EmailFailExeption : Exception
    {
        public EmailFailExeption() : base("Email Isn't Valid")
        {

        }
    }
}
