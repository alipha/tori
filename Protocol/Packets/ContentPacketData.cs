
using System;
using Protocol.Util;

namespace Protocol.Packets
{
    public class ContentPacketData : IPacketData
    {
        public byte[] Bytes { get; set; }   // 0 bytes would indicate to close the connection

        public void Read(ReadBuffer buffer, int length)
        {
            Bytes = buffer.ReadBytes(length);
        }

        public void Write(WriteBuffer buffer)
        {
            buffer.Write(Bytes);
        }
    }
}
