using System;
using Protocol.Util;

namespace Protocol.Packets
{
    public class RouteListPacketData : IPacketData
    {
        PacketRoute[] ReturnRoutes;


        public void ReadBytes(byte[] src, int srcIndex, int length)
        {
            var buffer = new ReadBuffer(src, srcIndex);

            var routeCount = length / PacketRoute.EncryptedSize;

            if(routeCount * PacketRoute.EncryptedSize != length)
                throw new Exception(string.Format("RouteListPacketData.ReadBytes: length {0} is not a multiple of {1}.", length, PacketRoute.EncryptedSize));

            ReturnRoutes = new PacketRoute[routeCount];

            for (var i = 0; i < routeCount; i++)
                ReturnRoutes[i] = new PacketRoute { EncryptedBytes = buffer.ReadBytes(PacketRoute.EncryptedSize) };
        }

        public int WriteBytes(byte[] dest, int destIndex)
        {
            var buffer = new WriteBuffer(dest, destIndex);

            foreach (var route in ReturnRoutes)
                buffer.Write(route.EncryptedBytes);

            return buffer.TotalWritten;
        }
    }
}
