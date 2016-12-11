using Socks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Protocol;
using Protocol.Util;
using Sodium;

namespace Tori
{
    public class ToriSocksConnection : ISocksConnection
    {
        private SocksConnectionInfo _info;
        private UdpClient _udpClient;


        public ToriSocksConnection(UdpClient udpClient)
        {

        }


        public bool Accept(SocksConnectionInfo info)
        {
            _info = info;

            var connection = new Connection
            {
                AddressTypeByte = (DestAddressType)(byte)_info.DestAddressType,
                DestAddress = _info.DestAddressType == SocksAddressType.DomainName ? Encoding.ASCII.GetBytes(_info.DestDomainName) : _info.DestIpAddress.GetAddressBytes(),
                DestPort = (ushort)_info.DestPort
            };

            var manager = new ConnectionManager(connection);
            return true;
        }

        public void Receive(TcpClient client)
        {
            /*
            var clientStream = client.GetStream();
            var RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);

            while (true)
            {
                var receiveBytes = _udpClient.Receive(ref RemoteIpEndPoint);
                clientStream.Write(receiveBytes, 0, receiveBytes.Length);
            }
            */

            // Blocks until a message returns on this socket from a remote

            /*

            _dest = new TcpClient(_info.DestIpAddress.GetAddressBytes().Length == 4 ? AddressFamily.InterNetwork : AddressFamily.InterNetworkV6);
            _dest.Connect(_info.DestIpAddress, _info.DestPort);

            _destStream = _dest.GetStream();
            _clientStream = client.GetStream();

            var task = new Task(Pipe, Tuple.Create(_clientStream, _destStream));
            task.Start();

            Pipe(_destStream, _clientStream);
             */
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
