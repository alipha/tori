
using System;

namespace Protocol
{
    public class Route
    {
        public int StartSequenceId { get; set; }

        public Tuple<byte[], NodeInfo>[] Nodes { get; set; }    // <SymmetricKey, Node>

        public Route OldRoute { get; set; }
    }
}
