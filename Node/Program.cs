using System.Net.Sockets;

namespace Node
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var port = 57120;

            if (args.Length > 0)
            {
                int argPort;
                if (int.TryParse(args[0], out argPort))
                    port = argPort;
            }

            var udpClient = new UdpClient(port);


        }
    }
}
