using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Interfaces
{
    public interface ISubscribe
    {
        Task Subscribe(string messageType, Action<object> action);
    }
}
