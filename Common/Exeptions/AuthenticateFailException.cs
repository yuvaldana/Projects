﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Exeptions
{
    public class AuthenticateFailException : Exception
    {

        public AuthenticateFailException() : base("Authentiction Failed")
        {       
        }
        public AuthenticateFailException(Exception innerException) : base("Authentication Failed", innerException)
        {
        }
    }
}
