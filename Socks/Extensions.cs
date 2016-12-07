using System.Net.Sockets;

namespace Socks
{
    public static class Extensions
    {
        public static bool ReadExactly(this NetworkStream stream, byte[] buffer)
        {
            int read = 0;

            while ((read < buffer.Length))
            {
                var amount = stream.Read(buffer, read, buffer.Length - read);

                if (amount <= 0)
                    return false;

                read += amount;
            }

            return true;
        }
    }
}
