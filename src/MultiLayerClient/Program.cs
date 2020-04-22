using System;
using Trinity;
using Trinity.Core.Lib;
using System.Collections.Generic;

namespace MultiLayerClient {
    class Program {
        static void Main(string[] args) {
            // Trinity doesn't load the config file correctly if we don't tell it to.
            TrinityConfig.LoadConfig();
            TrinityConfig.CurrentRunningMode = RunningMode.Client;

            Client client = new Client();

            //LoadGraph("/home/thiel/MultiLayerGE/data/journals/journals_config.txt");
            Console.WriteLine("Loading Graph...");
            LoadGraph("/home/thiel/MultiLayerGE/data/multiplex6/multiplex6_config.txt");
            Console.WriteLine("Graph Loaded");

            if (args[0] == "interactive") {
                client.RunInteractive();
            } else if(args[0] == "batch") {
                string fileName = args[1];
                client.RunBatch(fileName);
            }

            //Global.CloudStorage.SaveStorage();

            
            //Console.WriteLine("Loading from ge storage.");
            //Global.CloudStorage.LoadStorage();
            //Console.WriteLine("Finished Loading from ge storage.");
        }

        private static void LoadGraph (string configFilePath) {
            AlgorithmOptions algorithmOptions = new AlgorithmOptions(Timed:true);
            OutputOptions outputOptions = new OutputOptions(OutputType: OutputType.CSV);
            using (var msg = new LoadGraphProxyMessageWriter(algorithmOptions, outputOptions, configFilePath)) {
                MultiLayerProxy.MessagePassingExtension.LoadGraphProxy(Global.CloudStorage.ProxyList[0], msg);
            }
        }
    }
}
