
namespace Protocol.Packets
{
    public enum PacketDataType
    {
        Connect = 0,
        Content = 1,    // 0 bytes of content would indicate close
        Ack = 2,
        RouteList = 3,
        //Close = 4
    }
}
