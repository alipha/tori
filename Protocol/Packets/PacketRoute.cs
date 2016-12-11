using Protocol.Util;
using System;
using System.Collections.Generic;

namespace Protocol.Packets
{
    public class PacketRoute    // 120 + 82 = 202 (make the route be 5 hops? if you want less, then just leave dummy )
    {
        public const int MaxHopCount = 4;

        public const int EncryptedSize = 56 * MaxHopCount + 16;    // MaxHop=3: 184, MaxHop=4: 240


        public byte[] EphemeralPublicKey { get; set; }  // (32 bytes) used to compute shared secret with scalar mult 

        // Nonce: unkeyed hash of (ephemeral_pk || node_pk)

        public ulong DestNodeId { get; set; }    // 8 bytes

        // Either Next or (ClientId, ClientSignature) is null. If DestNodeId == 0, Next is Null. If DestNodeId != 0, ClientId is null
        public PacketRoute Next { get; set; }

        public byte[] ClientId { get; set; }   // 16 bytes. Provided by Entry Node when client connects to it via TCP.

        //public byte[] ClientSignature { get; set; }     // (64 bytes) Entry Node needs to validate Signature matches ClientId. Include hash(entry node's symmetric key) in the signature to prevent replays.

        public byte[] Mac { get; set; }     // (16 bytes) Need to auth to make sure a malicious node doesn't change the endpoint for a DoS attack. 
        // Actually, don't need to because the signature with replay preventation should prevent this?  Actually, I think you do. 

        
        public byte[] EncryptedBytes { get; set; }

        public void Encrypt(byte[] symmetricKey)
        {
            var buffer = new WriteBuffer();
        }

        public void Encrypt(IEnumerator<RouteNode> nodes)
        {
            var key = nodes.Current.SymmetricKey;
            var hasNext = nodes.MoveNext();

            if(Next == null && hasNext || Next != null && !hasNext)
                throw new Exception(string.Format("PacketRoute.Encrypt: hasNext is {0} but Next is{1} null.", hasNext, (Next == null ? " not" : "")));

            if (Next != null)
                Next.Encrypt(nodes);

            Encrypt(key);
        }
    }
}
