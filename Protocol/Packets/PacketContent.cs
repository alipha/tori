
namespace Protocol.Packets
{
    public class PacketContent
    {
        public byte[] ConnectionId { get; set; }    // 16 bytes

        public ulong SequenceId { get; set; }    // 8 bytes

        public uint Verifier { get; set; }   // 4 bytes

        public byte DataTypeByte { get; set; }  // 1 byte

        public PacketDataType DataType
        {
            get
            {
                return (PacketDataType)DataTypeByte;
            }
            set
            {
                DataTypeByte = (byte)value;
            }
        }

        public short DataLen { get; set; }  // 2 bytes

        public byte[] Data { get; set; }
    }
}
