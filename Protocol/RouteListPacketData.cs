using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol
{
    public class RouteListPacketData : IPacketData
    {
        PacketRoute[] Routes;


        public byte[] Bytes
        {
            get
            {
                return null;    // TODO
            }
        }
    }
}
