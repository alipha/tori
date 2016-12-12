
namespace Protocol.Packets
{
    public class Packet // overhead is about 406 bytes. max udp packet is 1280 (or 1500?). 9% - 32% overhead (8% - 28%)
    {
        public const int Size = 1280;

        //public bool IsFragment { get; set; }    // move this into DirectionAndRouteCount, but can't be encrypted. Can be used to discriminate packets. Bad idea.

        public byte[] Id { get; set; }  // on the outer layer only (16 bytes), null otherwise. Each node does a keyed hash of the Id value to get the next Id value. Starts with the SequenceId

        // this could be used to track packets
        //public byte FragmentId { get; set; }    // lower 4 bits is the sequence #. upper 4 bits is the (# of fragments - 1)

        // This needs to be inside the Route
        // Encrypt (this and EncryptedLen) by taking the keyed hash of the Id and xor the bottom 3 bytes with (this and the EncryptedLen)
        // public sbyte DirectionAndNodeCount { get; set; }    // if negative, towards client. if positive, towards exit node. NodeCount is bottom 3 bits (number of nodes in route)

        // If bottom-most bit is same as symmetric key's bottom-most, direction is towards exit node
        // If 2nd bottom-most bit is the same as symmetric key's 2nd bottom-most, DestNodeId == 0
        public byte DirectionAndIsFinalDest { get; set; }

        //public short EncryptedLen { get; set; }  // 2 bytes. Don't need. The encrypted len is always the remainder of the packet.

        public byte[] EphemeralPublicKey { get; set; }  // (32 bytes) used to compute shared secret with scalar mult 

        // Nonce: just use 1? //unkeyed hash of (ephemeral_pk || node_pk || Id)

        // All of the below is encrypted with the shared secret

        public ulong DestNodeId { get; set; }    // 8 bytes. null if towards client

        public PacketRoute Route { get; set; }   // null if towards Exit Node

        public Packet Layer { get; set; }   // Layer or Content is null. Layer is null if DestNodeId == 0. Content is null if DestNodeId != 0.

        public PacketContent Content { get; set; }

        public byte[] Mac { get; set; }     // 16 bytes (does this need to be on every layer to prevent malicious nodes from changing it to DoS clients or other nodes?)
    }
}
