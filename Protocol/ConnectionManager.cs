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
        private static IList<NodeInfo> _possibleNodes;
        private Connection _connection;

        public ConnectionManager(Connection connection)
        {
            _connection = connection;

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
