using System;
using Socks;
using System.Net.Sockets;

namespace Tori
{
    public class Program
    {
        public static void Main(string[] args)
        {
            /*
            var port = 57121;

            var udpClient = new UdpClient(port);
            var proxy = new SocksProxy(8080, () => new ToriSocksConnection(udpClient));
            */

            var proxy = new SocksProxy(8080, () => new BasicSocksConnection());
            proxy.Listen();
            Console.ReadLine();
        }
    }
}
