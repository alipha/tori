
namespace Socks
{
    public interface ISocksConnection : IConnection
    {
        bool Accept(SocksConnectionInfo info);
    }
}
