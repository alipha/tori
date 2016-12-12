using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using Protocol.Packets;
using Protocol.Util;
using Sodium;

namespace Protocol
{
    public class ConnectionManager
    {
        public static readonly byte[] ZeroNonce = { 0, 0, 0, 0, 0, 0, 0, 0 };
        //public static readonly byte[] OneNonce = { 0, 0, 0, 0, 0, 0, 0, 1};

        private static IList<NodeInfo> _possibleNodes;
        private Connection _connection;

        public ConnectionManager(Connection connection)
        {
            _connection = connection;
            _connection.UserId = SodiumCore.GetRandomBytes(8);
            _connection.ConnectionId = 0;
            _connection.NextSequenceId = 0;

            if (_possibleNodes == null)
            {
                var nodesJson = File.ReadAllText(@"C:\Users\Kevin.Spinar\Documents\Visual Studio 2015\Projects\tori\nodes.txt");
                _possibleNodes = new JavaScriptSerializer().Deserialize<NodeInfo[]>(nodesJson).ToList();
            }

            var routes = new Route[2];

            for (var i = 0; i < routes.Length; i++)
                routes[i] = GenerateRoute(_possibleNodes);

            _connection.ReturnRoutes = routes;
        }


        public ConnectPacketData GetConnectPacket()
        {
            var connectPacketData = new ConnectPacketData
            {
                AddressType = _connection.AddressType,
                DestPort = _connection.DestPort,
                RouteCount = (uint)_connection.ReturnRoutes.Length,
                ReturnRoutes = // TODO
            };

            if (_connection.AddressType == DestAddressType.DomainName)
                connectPacketData.DestDomainName = _connection.DestDomainName;
            else
                connectPacketData.DestIpAddress = _connection.DestIpAddress;

            var bytesToEncrypt = new byte[PacketContent.MaxSendDataLen + 27];

            var buffer = new WriteBuffer(bytesToEncrypt, 0);
            buffer.Write(_connection.UserId);
            buffer.Write(_connection.ConnectionId);
            buffer.Write(_connection.NextSequenceId);
            buffer.Write((byte)PacketDataType.Connect);
            buffer.Write(connectPacketData);
            var paddingLength = bytesToEncrypt.Length - buffer.TotalWritten;
            buffer.Write(new byte[paddingLength]);

            var route = GenerateRoute(_possibleNodes);
            var encrypted = SecretBox.Create(bytesToEncrypt, OneNonce, route.Nodes[2].SymmetricKey);

            var destIdAndDataLen = new byte[10];
            buffer.Buffer = destIdAndDataLen;
            buffer.Write((ulong)0);
            buffer.Write((ushort)(encrypted.Length - 16));

            var innerPacketBytes = new byte[bytesToEncrypt.Length + 67];
            buffer.Buffer = innerPacketBytes;
            
            buffer.Write(route.Nodes[2].Node.Id);
            buffer.Write(route.Nodes[2].EphemeralPublicKey);
            buffer.Write((byte)(route.Nodes[2].SymmetricKey[31] & 1));
            buffer.Write(StreamEncryption.EncryptChaCha20(destIdAndDataLen, ZeroNonce, route.Nodes[2].SymmetricKey));
            buffer.Write(encrypted);

            encrypted = SecretBox.Create(encrypted, OneNonce, route.Nodes[2].SymmetricKey);

            var destId = new byte[8];
            buffer.Buffer = destId;
            buffer.Write(route.Nodes[1].Node.Id);
        }


        private static Route GenerateRoute(IList<NodeInfo> possibleNodes, int startSequenceId = 0, Route oldRoute = null)
        {
            var nodes = new RouteNode[3];
            var i = 0;

            while (i < nodes.Length)
            {
                var nodeIndex = SodiumCore.GetRandomNumber(possibleNodes.Count);
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
            if (nodePrivateKey == null && ephemeralPrivateKey == null)
                throw new ArgumentNullException("nodePrivateKey", "Expected exactly one of nodePrivateKey or ephemeralPrivateKey to be null, but both were null.");

            if (nodePrivateKey != null && ephemeralPrivateKey != null)
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
    }
}
