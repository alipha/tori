
namespace Protocol.Packets
{
    public interface IPacketData
    {
        //byte[] Bytes { get; set; }

        void ReadBytes(byte[] src, int srcIndex, int length);

        int WriteBytes(byte[] dest, int destIndex); // returns how many bytes were written
    }
}
