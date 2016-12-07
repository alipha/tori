
using System;
namespace Protocol
{
    public class AckPacketData : IPacketData
    {
        public byte SubstituteRouteCount { get; set; }

        public Tuple<byte, PacketRoute>[] NewRoutes { get; set; }   // <Position of the route to replace, New route>

        public Tuple<long, uint>[] Verifiers { get; set; }  // <SequenceId, Verifier>


        public byte[] Bytes
        {
            get
            {
                return null;    // TODO
            }
        }
    }
}
