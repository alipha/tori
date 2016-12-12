
using System.Net;

namespace Protocol
{
    public class Connection
    {
        public byte[] UserId { get; set; }

        public ushort ConnectionId { get; set; }

        public ulong NextSequenceId { get; set; }

        public DestAddressType AddressType { get; set; }

        public IPAddress DestIpAddress { get; set; }

        public string DestDomainName { get; set; }

        public ushort DestPort { get; set; }

        public Route[] ReturnRoutes { get; set; }
    }
}
