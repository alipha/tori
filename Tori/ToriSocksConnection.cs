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
            CreateConnection();
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


        private void CreateConnection()
        {
            var nodesJson = File.ReadAllText(@"C:\Users\Kevin.Spinar\Documents\Visual Studio 2015\Projects\tori\nodes.txt");
            var nodes = new JavaScriptSerializer().Deserialize<NodeInfo[]>(nodesJson);
            
            var routes = new Route[2];

            for (var i = 0; i < routes.Length; i++)
                routes[i] = GenerateRoute(nodes);

            var connection = new Connection
            {
                AddressTypeByte = (DestAddressType)(byte)_info.DestAddressType,
                DestAddress = _info.DestAddressType == SocksAddressType.DomainName ? Encoding.ASCII.GetBytes(_info.DestDomainName) : _info.DestIpAddress.GetAddressBytes(),
                DestPort = (ushort)_info.DestPort,
                ReturnRoutes = routes
            };
            

        }


        private static Route GenerateRoute(NodeInfo[] possibleNodes, int startSequenceId = 0, Route oldRoute = null)
        {
            var nodes = new RouteNode[3];
            var i = 0;

            while (i < nodes.Length)
            {
                var nodeIndex = SodiumCore.GetRandomNumber(possibleNodes.Length);
                var node = possibleNodes[nodeIndex];

                if (nodes.Any(n => n != null && n.Node.Id == node.Id))
                    continue;

                var keyPair = PublicKeyBox.GenerateKeyPair();
                
                nodes[i++] = new RouteNode
                {
                    EphemeralPublicKey = keyPair.PublicKey,
                    SymmetricKey = GenerateSymmetricKey(node.PublicKey, null, keyPair.PublicKey, keyPair.PrivateKey),
                    Node = node
                };
            }

            return new Route
            {
                StartSequenceId = startSequenceId,
                OldRoute = oldRoute,
                Nodes = nodes
            };
        }


        private static byte[] GenerateSymmetricKey(byte[] nodePublicKey, byte[] nodePrivateKey, byte[] ephemeralPublicKey, byte[] ephemeralPrivateKey)
        {
            if(nodePrivateKey == null && ephemeralPrivateKey == null)
                throw new ArgumentNullException("nodePrivateKey", "Expected exactly one of nodePrivateKey or ephemeralPrivateKey to be null, but both were null.");

            if(nodePrivateKey != null && ephemeralPrivateKey != null)
                throw new ArgumentNullException("nodePrivateKey", "Expected exactly one of nodePrivateKey or ephemeralPrivateKey to be null, but both were provided.");

            var privateKey = nodePrivateKey ?? ephemeralPrivateKey;
            var publicKey = nodePrivateKey != null ? ephemeralPublicKey : nodePublicKey;

            var q = ScalarMult.Mult(privateKey, publicKey);

            var bytes = new byte[q.Length + ephemeralPublicKey.Length + nodePublicKey.Length];
            var writer = new WriteBuffer(bytes, 0);
            writer.Write(q);
            writer.Write(ephemeralPublicKey);
            writer.Write(nodePublicKey);
            return GenericHash.Hash(bytes, null, 32);
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
