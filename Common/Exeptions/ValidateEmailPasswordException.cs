﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Exeptions
{
    public class ValidateEmailPasswordException : Exception
    {
        public ValidateEmailPasswordException() : base("Failed to validate Email or Password with the Database")
        {

        }
    }
}
