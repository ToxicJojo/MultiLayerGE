using System;
using Trinity;

namespace MultiLayerClient {
    class Program {
        static void Main(string[] args) {
            // Trinity doesn't load the config file correctly if we don't tell it to.
            TrinityConfig.LoadConfig();
            TrinityConfig.CurrentRunningMode = RunningMode.Client;


            LoadGraph("/home/thiel/MultiLayerGE/data/multiplex6/multiplex6_config.txt");
            GetNodeCount();
            GetEdgeCount();
        }

        private static void LoadGraph (string configFilePath) {
            AlgorithmOptions algorithmOptions = new AlgorithmOptions(Timed:true);
            OutputOptions outputOptions = new OutputOptions(OutputType: OutputType.Console);
            using (var msg = new LoadGraphProxyMessageWriter(algorithmOptions, outputOptions, configFilePath)) {
                MultiLayerProxy.MessagePassingExtension.LoadGraphProxy(Global.CloudStorage.ProxyList[0], msg);
            }
        }

        private static void GetNodeCount () {
            AlgorithmOptions algorithmOptions = new AlgorithmOptions(Timed: true);
            OutputOptions outputOptions = new OutputOptions(OutputType: OutputType.Console);

            using (var msg = new StandardAlgorithmMessageWriter(algorithmOptions, outputOptions)) {
                MultiLayerProxy.MessagePassingExtension.GetNodeCountProxy(Global.CloudStorage.ProxyList[0], msg);
            }
        }

        private static void GetEdgeCount () {
            AlgorithmOptions algorithmOptions = new AlgorithmOptions(Timed: true);
            OutputOptions outputOptions = new OutputOptions(OutputType: OutputType.Console);

            using (var msg = new StandardAlgorithmMessageWriter(algorithmOptions, outputOptions)) {
                MultiLayerProxy.MessagePassingExtension.GetEdgeCountProxy(Global.CloudStorage.ProxyList[0], msg);
            }
        }
    }
}
