
namespace Socks.Exceptions
{
    public class UnsupportedSocksCommandException : SocksException
    {
        public SocksCommandCode ProvidedCommand;

        public UnsupportedSocksCommandException(SocksCommandCode provided)
            : base(string.Format("The provided SOCKS command code, {0}, is not supported. Only SocksCommandCode.Stream is supported.", provided))
        {
            ProvidedCommand = provided;
        }
    }
}
