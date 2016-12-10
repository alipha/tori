
namespace Protocol.Packets
{
    public class PacketContent
    {
        public byte[] UserId { get; set; }      // 16 bytes

        public ushort ConnectionId { get; set; }    // 2 bytes // old: 16 bytes

        public ulong SequenceId { get; set; }    // 8 bytes

        public uint Verifier { get; set; }   // 4 bytes. Maybe? Instead use a mac or something?

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
