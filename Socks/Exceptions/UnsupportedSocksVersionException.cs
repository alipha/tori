
namespace Socks.Exceptions
{
    public class UnsupportedSocksVersionException : SocksException
    {
        public int ProvidedVersion;

        public UnsupportedSocksVersionException(int provided)
            : base(string.Format("The provided SOCKS version, {0}, is not supported. Only version 5 is supported.", provided))
        {
            ProvidedVersion = provided;
        }
    }
}
