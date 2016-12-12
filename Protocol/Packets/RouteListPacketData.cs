using System;
using Protocol.Util;

namespace Protocol.Packets
{
    public class RouteListPacketData : IPacketData
    {
        PacketRoute[] ReturnRoutes;


        public void Read(ReadBuffer buffer, int length)
        {
            var routeCount = length / PacketRoute.EncryptedSize;

            if(routeCount * PacketRoute.EncryptedSize != length)
                throw new Exception(string.Format("RouteListPacketData.ReadBytes: length {0} is not a multiple of {1}.", length, PacketRoute.EncryptedSize));

            ReturnRoutes = new PacketRoute[routeCount];

            for (var i = 0; i < routeCount; i++)
                ReturnRoutes[i] = new PacketRoute { EncryptedBytes = buffer.ReadBytes(PacketRoute.EncryptedSize) };
        }

        public void Write(WriteBuffer buffer)
        {
            foreach (var route in ReturnRoutes)
                buffer.Write(route.EncryptedBytes);
        }
    }
}
