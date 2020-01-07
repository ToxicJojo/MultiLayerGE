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
            System.Threading.Thread.Sleep(5000);
            int[] nodeCount = proxy.GetNodeCount();
            for (int i = 0; i < nodeCount.Length; i++) {
                Console.WriteLine("[Layer {0}] {1} Nodes", i + 1, nodeCount[i]);
            }

            Console.ReadLine();
            proxy.Stop();
        }
    }
}
