using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socks.Exceptions
{
    public class SocksDnsException : SocksException
    {
        public string DomainName;


        public SocksDnsException(string domainName)
            : base(string.Format("DNS lookup for {0} failed.", domainName))
        {
            DomainName = domainName;
        }
    }
}
