using System;
using Trinity;
using MultiLayerProxy.Proxy;

namespace MultiLayerProxy {
    class Program {
        static void Main(string[] args) {
            TrinityConfig.LoadConfig();
            
            MultiLayerProxyImpl proxy = new MultiLayerProxyImpl();
            proxy.Start();

            Console.ReadLine();
            proxy.Stop();
        }
    }
}
