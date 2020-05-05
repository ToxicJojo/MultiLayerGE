using System;
using Trinity;
using Trinity.Network;
using MultiLayerServer.Server;

namespace MultiLayerServer {
    class Program {
        private static string StorageRootBasePath = "/home/thiel/ge-storage/server";
        static void Main(string[] args) {
            TrinityConfig.LoadConfig();
            // Make sure servers use different folders to store their data.
            TrinityConfig.StorageRoot = StorageRootBasePath + args[0];

            TrinityServer server = new MultiLayerServerImpl();
            server.Start();

            Console.ReadLine();
            server.Stop();
        }
    }
}
