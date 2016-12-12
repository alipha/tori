
using Protocol.Util;

namespace Protocol.Packets
{
    public interface IPacketData
    {
        //byte[] Bytes { get; set; }

        void Read(ReadBuffer buffer, int length);

        void Write(WriteBuffer buffer);
    }
}
