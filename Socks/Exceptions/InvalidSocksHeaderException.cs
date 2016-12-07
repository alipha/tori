using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socks.Exceptions
{
    public class InvalidSocksHeaderException : SocksException
    {
        public InvalidSocksHeaderException(string message) : base(string.Format("The client did not provide a valid SOCKS header. {0}", message)) { }
    }
}
