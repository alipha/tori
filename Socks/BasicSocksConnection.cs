using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Socks
{
    public class BasicSocksConnection : ISocksConnection
    {
        private SocksConnectionInfo _info;
        private TcpClient _dest;
        private NetworkStream _destStream;
        private NetworkStream _clientStream;


        public TcpClient Dest { get { return _dest; } }

        public TcpClient Client { get { return _info.Client; } }


        public bool Accept(SocksConnectionInfo info)
        {
            _info = info;

            var clientEndPoint = (IPEndPoint)info.Client.Client.RemoteEndPoint;
            Console.WriteLine("New connection from {0}:{1} to {2}:{3}", clientEndPoint.Address, clientEndPoint.Port, info.DestIpAddress, info.DestPort);

            return true;
        }


        public void Receive(TcpClient client)
        {
            _dest = new TcpClient(_info.DestIpAddress.GetAddressBytes().Length == 4 ? AddressFamily.InterNetwork : AddressFamily.InterNetworkV6);
            _dest.Connect(_info.DestIpAddress, _info.DestPort);

            _destStream = _dest.GetStream();
            _clientStream = client.GetStream();

            var task = new Task(Pipe, Tuple.Create(_clientStream, _destStream));
            task.Start();

            Pipe(_destStream, _clientStream);
        }


        private void Pipe(object streamObject)
        {
            var streams = (Tuple<NetworkStream, NetworkStream>)streamObject;

            Pipe(streams.Item1, streams.Item2);
        }

        private void Pipe(NetworkStream readStream, NetworkStream writeStream)
        {
            try
            {
                var buffer = new byte[1000];
                int amount;

                while ((amount = readStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    writeStream.Write(buffer, 0, amount);
                }

                Console.WriteLine("Connection closed.");
            }
            catch { }
        }
    }
}
