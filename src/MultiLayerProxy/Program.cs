using System;
using Trinity;

namespace MultiLayerProxy
{
    class Program
    {
        static void Main(string[] args)
        {
            TrinityConfig.LoadConfig();

            MultiLayerProxyImpl proxy = new MultiLayerProxyImpl();
            proxy.Start();


            proxy.LoadGraphHandler();
            int[] nodeCount = proxy.GetNodeCount();
            for (int i = 0; i < nodeCount.Length; i++) {
                Console.WriteLine("[Layer {0}] {1} Nodes", i + 1, nodeCount[i]);
            }
            
            int[] edgeCount = proxy.GetEdgeCount();
            for (int i = 0; i < edgeCount.Length; i++) {
                Console.WriteLine("[Layer {0}] {1} Edges", i + 1, edgeCount[i]);
            }

            Console.ReadLine();
            proxy.Stop();
        }
    }
}
