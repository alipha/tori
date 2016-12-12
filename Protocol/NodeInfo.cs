using System.Linq;
using System.Net;

namespace Protocol
{
    public class NodeInfo
    {
        private byte[] _ipAddress;
        private string _domainName;


        public ulong Id;

        public byte[] Address
        {
            get
            {
                if (_ipAddress == null)
                {
                    var ipAddress = Dns.GetHostAddresses(DomainName).FirstOrDefault();
                    _ipAddress = ipAddress != null ? ipAddress.GetAddressBytes() : null;
                }
                return _ipAddress;
            }
            set
            {
                DomainName = null;
                _ipAddress = value;
            }
        }

        public string DomainName
        {
            get
            {
                return _domainName;
            }
            set
            {
                _ipAddress = null;
                _domainName = value;
            }
        }

        public int Port;

        public byte[] PublicKey;
    }
}
