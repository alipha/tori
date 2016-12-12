using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using Protocol;
using Protocol.Util;
using Sodium;
using System.Web.Script.Serialization;

namespace Node
{
    public class Program
    {
        private static string[] _privateKeys =
        {
            "tEP3c79h+0n9eXPvp/q/z6iAvsF5hJySVmH+aQEX/cw=",
            "L1qh0pZldx8jV7xElRhPVzzUPK/pFpqJ9lHnzvukh/U=",
            "Rr7Dai9JSa35WtI11fH13KxNZ7HiCSHXoI/fXEdnfpg=",
            "oWPd3OACCMwobQIQWww8/UXVzuoC/0BGXpgnqGXNvUs=",
            "KzGeXjxEFkShLaMHg+zkxqzVI/IwFiLRXpeC/sg6ibU=",
            "yEeXDXxOsca/GkOq19Dc6bq43LMVe4CvZFvrukKvgLg=",
            "0ijxkt/PENT0A3aOz2TpeE0axoLUx9dFHLbr4J+X2CQ=",
            "vv04lyNd3EDUsP6FGbF46zgJKYdSfI6sChRbYVrDquM=",
            "INsbHvBRtA7VbKMRo7WQ1cFH90wPja9yMahpdBYojng=",
            "/wpW94AZtRcGVu+AC1PozZ83dBlE6M+q11FtD9HtXjc=",
            "dPPT0VQduPQrFJVKAgwn1cePGxj5iJUlpZeTiLO+23Q=",
            "5g49ZwDy1LZlVpRewSg4SKkHNvOGnKTyfOkEU6C8dVc=",
            "dJdlICC6g/+F8SDbb64i47opmAD39aKgKSS83x3UqfM=",
            "Efnprv6b4uXmzlM0OCAx1KfVNVkPxuWeLAuKROUGdFk=",
            "mOdqt+7nnm7uSrb6zEyWbD2/XkveKEOBd41QTtY2w6k=",
            "+Pl2mFIactMcG1dvUxWMFPrADWDfm21j9+a+YOfzCFU=",
            "U+qE86e5e7YZZHWDkZ/ch1aPPb8TQYbWG+j0TPwF4fk=",
            "xi2qhtwWZ8hQodTw7oOw/j3KbolRcO5V6QDziYJJ7V0=",
            "4K885bIu6fGI0kL9Qd5B12G4RAEykH3548sthf6NlwQ=",
            "2ZvkfY02ufzyetEl0flhVfdIcCk7JPXshDs9ecaneJg=",
            "u2AzzlRdV6WX5rY0rxottS30uzchRS3Ns6ZR+HbE30g=",
            "OfmrC5BKayWw5p5DsdtxCV/C79/X6ov23d7KfDp4qtQ=",
            "540ID7YOJIEjUUGt/1w+oHGet+a3Hk4/kxbnTvoR0Qg=",
            "/C9bp8/hKlNlDfDePquEXjEvcDbDBRSJ8ys2fndPYQk=",
            "yGY+tzVz3JrXuzynEWgyp7eN8zjXf/ogtbQOypGGKTs=",
            "6+uJvZ7rPDFfq6MobAnfgDSauIqya6iK5bO8T6O3erk=",
            "sEuSRplvjJcbHsb09AdNojhwJjNIoCQqXBrOiahkLpU=",
            "jUtNX6H49sUSl7mMJsILSmcMh+tViBnhS3hLhZmqBak=",
            "IjwkihMDlqUh7LI7D7hxy999OfTRXFFlpciFa9y70aY=",
            "MGcARpueWxJaHA636GGLM/r63hjXlWQaoiE1CKJUDD0="
        };

        public static void Main(string[] args)
        {
            SodiumCore.Init();

            /*
            var port = 57120;

            if (args.Length > 0)
            {
                int argPort;
                if (int.TryParse(args[0], out argPort))
                    port = argPort;
            }

            var udpClient = new UdpClient(port);
            */


            GenerateNodeKeys();
            Console.ReadLine();
        }


        public static void GenerateNodeKeys()
        {
            var rng = new RNGCryptoServiceProvider();
            var id = new byte[8];
            var idReader = new ReadBuffer(id, 0);
            var nodes = new NodeInfo[30];

            for (var i = 0; i < nodes.Length; i++)
            {
                var keyPair = PublicKeyBox.GenerateKeyPair();
                rng.GetBytes(id);

                nodes[i] = new NodeInfo
                {
                    Address = IPAddress.Any.GetAddressBytes(),
                    Port = 57120 + i,
                    PublicKey = keyPair.PublicKey
                };

                idReader.Read(out nodes[i].Id);
                idReader.InitialPosition = 0;
                Console.WriteLine("\"{0}\",", Convert.ToBase64String(keyPair.PrivateKey));
            }

            //File.WriteAllText(@"C:\Users\Kevin.Spinar\Documents\Visual Studio 2015\Projects\tori\nodes.txt", new JavaScriptSerializer().Serialize(nodes));
        }
    }
}
