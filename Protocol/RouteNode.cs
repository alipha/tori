
namespace Protocol
{
    public class RouteNode
    {
        public byte[] EphemeralPublicKey { get; set; }

        public byte[] SymmetricKey { get; set; }

        public NodeInfo Node { get; set; }
    }
}
