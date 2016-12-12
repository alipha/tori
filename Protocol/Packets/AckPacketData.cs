using System;
using Protocol.Util;

namespace Protocol.Packets
{
    public class AckPacketData : IPacketData
    {
        public const int VerifierSize = 12;


        public byte SubstituteRouteCount;  // if any routes are provided, the exit node will have to ACK

        public Tuple<uint, PacketRoute>[] NewRoutes;   // <Position of the route to replace, New route>

        public Tuple<ulong, uint>[] Verifiers;  // <SequenceId, Verifier>
        // Might add some way to authenticate the Id was set correctly and the padding
        // Or just a simple: LowSequenceId, HighSequenceId?


        public void Read(ReadBuffer buffer, int length)
        {
            buffer.Read(out SubstituteRouteCount);

            NewRoutes = new Tuple<uint, PacketRoute>[SubstituteRouteCount];

            for (var i = 0; i < SubstituteRouteCount; i++)
            {
                uint position;
                buffer.Read(out position);

                NewRoutes[i] = new Tuple<uint, PacketRoute>(
                    position,
                    new PacketRoute { EncryptedBytes = buffer.ReadBytes(PacketRoute.EncryptedSize) }
                );
            }

            var remainingLength = length - buffer.TotalRead;

            var verifierCount = remainingLength / VerifierSize;

            if (verifierCount * VerifierSize != remainingLength)
                throw new Exception(string.Format("AckPacketData.ReadBytes: remaining length {0} is not a multiple of {1}.", remainingLength, VerifierSize));

            Verifiers = new Tuple<ulong, uint>[verifierCount];

            for (var r = 0; r < verifierCount; r++)
            {
                ulong sequenceId;
                uint verifier;
                buffer.Read(out sequenceId);
                buffer.Read(out verifier);
                Verifiers[r] = new Tuple<ulong, uint>(sequenceId, verifier);
            }
        }

        public void Write(WriteBuffer buffer)
        {
            buffer.Write(SubstituteRouteCount);

            foreach (var newRoute in NewRoutes)
            {
                buffer.Write(newRoute.Item1);
                buffer.Write(newRoute.Item2.EncryptedBytes);
            }

            foreach (var verifier in Verifiers)
            {
                buffer.Write(verifier.Item1);
                buffer.Write(verifier.Item2);
            }
        }
    }
}
