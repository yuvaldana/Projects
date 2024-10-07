using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Exeptions
{
    public class SavingOrUpdatingDBException : Exception
    {
        public SavingOrUpdatingDBException() : base("Saving Or Updating Context to the DataBase Has Faild")
        { }
    }
}
