using System;
using Protocol.Util;

namespace Protocol.Packets
{
    public class AckPacketData : IPacketData
    {
        public const int VerifierSize = 12;


        public byte SubstituteRouteCount { get; set; }

        public Tuple<byte, PacketRoute>[] NewRoutes { get; set; }   // <Position of the route to replace, New route>

        public Tuple<ulong, uint>[] Verifiers { get; set; }  // <SequenceId, Verifier>
        // Might add some way to authenticate the Id was set correctly and the padding


        public void ReadBytes(byte[] src, int srcIndex, int length)
        {
            var buffer = new ReadBuffer(src, srcIndex);
            SubstituteRouteCount = buffer.ReadByte();

            NewRoutes = new Tuple<byte, PacketRoute>[SubstituteRouteCount];

            for (var i = 0; i < SubstituteRouteCount; i++)
            {
                var position = buffer.ReadByte();

                NewRoutes[i] = new Tuple<byte, PacketRoute>(
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
                var sequenceId = buffer.ReadULong();
                Verifiers[r] = new Tuple<ulong, uint>(sequenceId, buffer.ReadUInt());
            }
        }

        public int WriteBytes(byte[] dest, int destIndex)
        {
            var buffer = new WriteBuffer(dest, destIndex);
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

            return buffer.TotalWritten;
        }
    }
}
