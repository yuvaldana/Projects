﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Exeptions
{
    public class PasswordFailExeption : Exception
    {
        public PasswordFailExeption() : base("Password Isn't Strong Enough")
        {
              
        }
    }
}
