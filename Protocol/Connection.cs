
namespace Protocol
{
    public class Connection
    {
        public DestAddressType AddressTypeByte { get; set; }

        public byte[] DestAddress { get; set; }

        public ushort DestPort { get; set; }

        public Route[] ReturnRoutes { get; set; }
    }
}
