using System;
using Trinity;

namespace MultiLayerClient
{
    class Program
    {
        static void Main(string[] args)
        {
            // Trinity doesn't load the config file correctly if we don't tell it to.
            TrinityConfig.LoadConfig();
            TrinityConfig.CurrentRunningMode = RunningMode.Client;


            Global.CloudStorage.SaveNode(0, 42);

            Node node = Global.CloudStorage.LoadNode(0);
            Console.WriteLine("Node Value: {0}", node.Value);
        }
    }
}
