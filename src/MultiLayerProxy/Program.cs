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

            Console.ReadLine();
            proxy.Stop();
        }
    }
}
