using System.Net;
using System.Net.Sockets;

namespace Socks
{
    public class SocksConnectionInfo
    {
        public SocksProxy Proxy;
        public TcpClient Client;
        public int Version;
        public SocksCommandCode Command;
        public SocksAddressType DestAddressType;
        public int DestPort;
        public IPAddress DestIpAddress;
        public string DestDomainName;
    }
}
