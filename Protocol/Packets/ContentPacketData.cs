
using System;

namespace Protocol.Packets
{
    public class ContentPacketData : IPacketData
    {
        public byte[] Bytes { get; set; }   // 0 bytes would indicate to close the connection

        public void ReadBytes(byte[] src, int srcIndex, int length)
        {
            Bytes = new byte[length];
            Array.Copy(src, srcIndex, Bytes, 0, length);
        }

        public int WriteBytes(byte[] dest, int destIndex)
        {
            Bytes.CopyTo(dest, destIndex);
            return Bytes.Length;
        }
    }
}
