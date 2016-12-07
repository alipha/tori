
namespace Socks.Exceptions
{
    public class UnsupportedSocksAuthMethodException : SocksException
    {
        public SocksAuthMethodCode[] ProvidedAuthMethod;

        public UnsupportedSocksAuthMethodException(SocksAuthMethodCode[] provided)
            : base(string.Format("The provided SOCKS authentication methods ({0}) are not supported. Only AuthMethodCode.None is supported.", string.Join(", ", provided)))
        {
            ProvidedAuthMethod = provided;
        }
    }
}
