
using System;

namespace Protocol
{
    public class Route
    {
        public int StartSequenceId { get; set; }

        public RouteNode[] Nodes { get; set; }

        public Route OldRoute { get; set; }
    }
}
