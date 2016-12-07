using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Socks.Exceptions;


namespace Socks
{
    public class SocksProxy
    {
        private static readonly byte[] _successReply = { 5, 0 };

        private readonly int _port;
        private readonly Func<ISocksConnection> _factory;
        private readonly HashSet<ISocksConnection> _connections;
        private volatile bool _listening;


        public int Port { get { return _port; } }

        public HashSet<ISocksConnection> Connections { get { return _connections; } }


        public SocksProxy(int port, Func<ISocksConnection> connectionFactory)
        {
            _port = port;
            _factory = connectionFactory;
            _connections = new HashSet<ISocksConnection>();
            _listening = false;
        }

        public void Listen()
        {
            _listening = true;
            var task = new Task(AcceptLoop);
            task.Start();
        }

        public void Close()
        {
            _listening = false;
        }


        private void AcceptLoop()
        {
            var listener = new TcpListener(IPAddress.Any, Port);
            listener.Start();

            while(_listening) 
            {
                var client = listener.AcceptTcpClient();
                var clientTask = new Task(ProcessConnection, client);
                clientTask.Start();
            }
        }

        private void ProcessConnection(object clientObject)
        {
            //int b;
            var user = new StringBuilder();
            var client = (TcpClient)clientObject;

            var stream = client.GetStream();
            //Version4();

            try
            {
                // Parse Version
                var version = stream.ReadByte();

                if (version == -1)
                    return;

                if (version != 5)
                    throw new UnsupportedSocksVersionException(version);


                // Parse Authentication Method
                var authMethodCount = stream.ReadByte();

                if (authMethodCount == 0)
                    throw new InvalidSocksHeaderException(string.Format("Authentication Method Count was {0}", authMethodCount));
                else if(authMethodCount < 0)
                    return;

                var authMethods = new byte[authMethodCount];
                if(!stream.ReadExactly(authMethods))
                    return;

                if (!authMethods.Any(x => x == (int)SocksAuthMethodCode.None))
                    throw new UnsupportedSocksAuthMethodException(authMethods.Select(x => (SocksAuthMethodCode)x).ToArray());

                stream.Write(_successReply, 0, _successReply.Length);


                // Parse Command Code
                var header = new byte[4];
                if(!stream.ReadExactly(header))
                    return;

                var info = new SocksConnectionInfo
                {
                    Proxy = this,
                    Client = client,
                    Version = header[0],
                    Command = (SocksCommandCode)header[1],
                    DestAddressType = (SocksAddressType)header[3]
                };

                if(info.Version != 5)
                    throw new UnsupportedSocksVersionException(version);

                if (info.Command != SocksCommandCode.Stream)
                    throw new UnsupportedSocksCommandException(info.Command);

                if (header[2] != 0)
                    throw new InvalidSocksHeaderException(string.Format("Expected reserved field to be 0 and not {0}.", header[2]));


                // Parse Destination Address Type
                switch (info.DestAddressType)
                {
                    case SocksAddressType.IPv4:
                        var ipv4 = new byte[4];
                        if(!stream.ReadExactly(ipv4))
                            return;

                        info.DestIpAddress = new IPAddress(ipv4);
                        break;

                    case SocksAddressType.IPv6:
                        var ipv6 = new byte[16];
                        if(!stream.ReadExactly(ipv6))
                            return;

                        info.DestIpAddress = new IPAddress(ipv6);
                        break;

                    case SocksAddressType.DomainName:
                        var domainNameLen = stream.ReadByte();

                        if(domainNameLen == 0)
                            throw new InvalidSocksHeaderException(string.Format("Unexpected domain name length, {0}.", domainNameLen));
                        else if(domainNameLen < 0)
                            return;

                        var domainName = new byte[domainNameLen];

                        if(!stream.ReadExactly(domainName))
                            return;

                        info.DestDomainName = Encoding.ASCII.GetString(domainName);
                        info.DestIpAddress = Dns.GetHostAddresses(info.DestDomainName).FirstOrDefault();

                        if (info.DestIpAddress == null)
                            throw new SocksDnsException(info.DestDomainName);
                        break;

                    default:
                        throw new InvalidSocksHeaderException(string.Format("Unexpected address Type {0}.", (int)info.DestAddressType));
                }


                // Parse Destination Port
                var port = new byte[2];

                if (!stream.ReadExactly(port))
                    return;

                info.DestPort = port[0] * 256 + port[1];


                // Reply Success
                stream.Write(_successReply, 0, _successReply.Length);
                stream.WriteByte(0);

                var addressBytes = info.DestIpAddress.GetAddressBytes();
                stream.WriteByte((byte)(addressBytes.Length == 4 ? SocksAddressType.IPv4 : SocksAddressType.IPv6));
                stream.Write(addressBytes, 0, addressBytes.Length);
                stream.Write(port, 0, port.Length);


                var connection = _factory();

                if (!connection.Accept(info))
                    return;

                _connections.Add(connection);
                connection.Receive(client);
            }
            finally
            {
                client.Close();
            }
        }


        /*
        private void Version4()
        {
                var header = new byte[4];
                var ipAddress = new byte[4];
                stream.Read(header, 0, header.Length);

                var info = new SocksConnectionInfo
                {
                    Proxy = this,
                    Client = client,
                    Version = header[0],
                    Command = (SocksCommandCode)header[1],
                    DestPort = header[2] * 256 + header[3],
                    DestIpAddress = new IPAddress(ipAddress)
                };

                if (info.Command != SocksCommandCode.Stream)
                    throw new UnsupportedSocksCommandException(info.Command);

                while ((b = stream.ReadByte()) > 0 && user.Length < 1000)
                    user.Append((char)b);

                if (b != 0)
                    throw new InvalidSocksHeaderException("Username was of unexpected length.");

                info.User = user.ToString();
        }
         */
    }
}
