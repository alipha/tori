using System;

namespace Protocol.Util
{
    public class ReadBuffer
    {
        private byte[] _buffer;
        private int _initialPosition;
        private int _position;

        public byte[] Buffer { get { return _buffer;  } }

        public int InitialPosition
        {
            get { return _initialPosition; }
            set { _position = _initialPosition = value; }
        }

        public int Position { get { return _position; } }

        public int TotalRead { get { return _position - _initialPosition; } }


        public ReadBuffer(byte[] buffer, int initialPosition)
        {
            _buffer = buffer;
            InitialPosition = initialPosition;
        }

        public byte[] ReadBytes(int count)
        {
            var dest = new byte[count];
            Array.Copy(_buffer, _position, dest, 0, count);
            _position += count;
            return dest;
        }

        public void Read(out byte value)
        {
            value = _buffer[_position++];
        }

        public void Read(out ushort value)
        {
            value = (ushort) ((_buffer[_position] << 8) | _buffer[_position + 1]);
            _position += 2;
        }

        public void Read(out uint value)
        {
            value = (uint)((_buffer[_position] << 24) | (_buffer[_position + 1] << 16) | (_buffer[_position + 2] << 8) | _buffer[_position + 3]);
            _position += 4;
        }

        public void Read(out ulong value)
        {
            uint topValue, bottomValue;
            Read(out topValue);
            Read(out bottomValue);
            value = ((ulong)topValue << 32) | bottomValue;
        }
    }
}
