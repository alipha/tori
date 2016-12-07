
namespace Protocol
{
    public class ContentPacketData : IPacketData
    {
        public byte[] Bytes { get; set; }   // 0 bytes would indicate to close the connection
    }
}
