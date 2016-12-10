
namespace Protocol.Util
{
    public class WriteBuffer
    {
        private readonly byte[] _buffer;
        private readonly int _initialPosition;
        private int _position;

        public byte[] Buffer { get { return _buffer; } }

        public int InitialPosition { get { return _initialPosition; } }

        public int Position { get { return _position; } }

        public int TotalWritten { get { return _position - _initialPosition; } }


        public WriteBuffer(byte[] buffer, int initialPosition)
        {
            _buffer = buffer;
            _position = _initialPosition = initialPosition;
        }

        public void Write(byte[] src)
        {
            var count = src.Length;
            src.CopyTo(_buffer, _position);
            _position += count;
        }

        public void Write(byte value)
        {
            _buffer[_position++] = value;
        }

        public void Write(ushort value)
        {
            _buffer[_position++] = (byte)(value >> 8);
            _buffer[_position++] = (byte)(value & 255);
        }

        public void Write(uint value)
        {
            Write((ushort)(value >> 16));
            Write((ushort)(value & ushort.MaxValue));
        }

        public void Write(ulong value)
        {
            Write((uint)(value >> 32));
            Write((uint)(value & uint.MaxValue));
        }
    }
}
