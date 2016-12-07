using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Node
{
    public class NodeInfo
    {
        private IPAddress _ipAddress;
        private string _domainName;


        public long Id { get; set; }

        public IPAddress Address
        {
            get
            {
                if (_ipAddress == null)
                    _ipAddress = Dns.GetHostAddresses(DomainName).FirstOrDefault();
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

        public int Port { get; set; }

        public byte[] PublicKey { get; set; }
    }
}
