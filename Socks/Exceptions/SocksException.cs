using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socks.Exceptions
{
    public class SocksException : Exception
    {
        public SocksException(string message) : base(message) { }
    }
}
