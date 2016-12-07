
namespace Protocol
{
    public class PacketRoute    // 120 + 82 = 202 (make the route be 5 hops? if you want less, then just encrypt your packets with multiple keys)
    {
        public byte[] EphemeralPublicKey { get; set; }  // (32 bytes) used to compute shared secret with scalar mult 

        // Nonce: Keyed hash of (ephemeral_pk || node_pk)

        public ulong DestNodeId { get; set; }    // 8 bytes

        public uint ClientId { get; set; }   // 4 bytes. Provided by Entry Node when client connects to it via TCP.

        public byte[] ClientSignature { get; set; }     // (64 bytes) Entry Node needs to validate Signature matches ClientId. Include hash(entry node's symmetric key) in the signature to prevent replays.

        public byte[] Mac { get; set; }     // Need to auth to make sure a malicious node doesn't change the endpoint for a DoS attack. 
        // Actually, don't need to because the signature with replay preventation should prevent this?  Actually, I think you do. 
    }
}
