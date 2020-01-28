using System;
using Trinity;
using Trinity.Network;
using MultiLayerServer.Loading;

namespace MultiLayerServer
{
    class Program
    {
        private static string StorageRootBasePath = "/home/thiel/ge-storage/server";
        static void Main(string[] args)
        {
            TrinityConfig.LoadConfig();
            // Make sure servers use different folders to store their data.
            TrinityConfig.StorageRoot = StorageRootBasePath + args[0];

            TrinityServer server = new MultiLayerServerImpl();
            server.Start();


            string configFilePath = "/home/thiel/MultiLayerGE/data/multiplex6/multiplex6_config.txt";
            GraphLoader graphLoader = new GraphLoader(new MultiLayerDirectedWeightedEdgeLoader());
            graphLoader.LoadGraph(configFilePath);


            Console.ReadLine();
            server.Stop();
        }

        private static void LogNodeInfo(long cellId) {
          Node node = Graph.LoadNode(cellId);
          Console.WriteLine("---NodeInfo---");
          Console.WriteLine("NodeId: {0}", cellId);
          Console.WriteLine("Layer: {0}", node.Layer);
          Console.WriteLine("EdgeCount: {0}", node.Edges.Count);
        }
    }
}
