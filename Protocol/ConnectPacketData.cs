using System.Net;
using System.Text;

namespace Protocol
{
    public class ConnectPacketData : IPacketData
    {
        public byte AddressTypeByte { get; set; }   // combine with RouteCount?

        public byte[] DestAddress { get; set; }

        public ushort DestPort { get; set; }

        public byte RouteCount { get; set; }    // must be at least 1. can be greater than the number of routes (in which case, expect another route packet)

        public PacketRoute[] Routes { get; set; }


        public IPAddress DestIpAddress
        {
            get
            {
                if (AddressType == DestAddressType.DomainName || DestAddress == null)
                    return null;
                return new IPAddress(DestAddress);
            }
            set
            {
                DestAddress = value.GetAddressBytes();
                AddressTypeByte = (byte)(DestAddress.Length == 4 ? DestAddressType.IPv4 : DestAddressType.IPv6);
            }
        }

        public string DestDomainName
        {
            get
            {
                if (AddressType != DestAddressType.DomainName || DestAddress == null)
                    return null;
                return Encoding.ASCII.GetString(DestAddress);
            }
            set
            {
                DestAddress = Encoding.ASCII.GetBytes(value);
                AddressTypeByte = (byte)DestAddressType.DomainName;
            }
        }

        public DestAddressType AddressType
        {
            get
            {
                return (DestAddressType)AddressTypeByte;
            }
            set
            {
                AddressTypeByte = (byte)value;
            }
        }

        public byte[] Bytes
        {
            get
            {
                return null;    // TODO
            }
        }
    }
}
