using System;
using Trinity;

namespace MultiLayerClient {
    class Program {
        static void Main(string[] args) {
            // Trinity doesn't load the config file correctly if we don't tell it to.
            TrinityConfig.LoadConfig();
            TrinityConfig.CurrentRunningMode = RunningMode.Client;


            LoadGraph("/home/thiel/MultiLayerGE/data/multiplex6/multiplex6_config.txt");
            //GetNodeCount();
            //GetEdgeCount();
            HITS(1, 0.5, false);
            HITSTopAuthorities(1,true);
            HITSTopHubs(5,true);
            //PageRank(1, 0.35, true);
            //PageRankTopNodes(5, true);
        }

        private static void LoadGraph (string configFilePath) {
            AlgorithmOptions algorithmOptions = new AlgorithmOptions(Timed:true);
            OutputOptions outputOptions = new OutputOptions(OutputType: OutputType.CSV);
            using (var msg = new LoadGraphProxyMessageWriter(algorithmOptions, outputOptions, configFilePath)) {
                MultiLayerProxy.MessagePassingExtension.LoadGraphProxy(Global.CloudStorage.ProxyList[0], msg);
            }
        }

        private static void GetNodeCount () {
            AlgorithmOptions algorithmOptions = new AlgorithmOptions(Timed: true);
            OutputOptions outputOptions = new OutputOptions(OutputType: OutputType.CSV);

            using (var msg = new StandardAlgorithmMessageWriter(algorithmOptions, outputOptions)) {
                MultiLayerProxy.MessagePassingExtension.GetNodeCountProxy(Global.CloudStorage.ProxyList[0], msg);
            }
        }

        private static void GetEdgeCount () {
            AlgorithmOptions algorithmOptions = new AlgorithmOptions(Timed: true);
            OutputOptions outputOptions = new OutputOptions(OutputType: OutputType.CSV);

            using (var msg = new StandardAlgorithmMessageWriter(algorithmOptions, outputOptions)) {
                MultiLayerProxy.MessagePassingExtension.GetEdgeCountProxy(Global.CloudStorage.ProxyList[0], msg);
            }
        }

        private static void PageRank (double initalValue, double epsilon, bool seperateLayers) {
            AlgorithmOptions algorithmOptions = new AlgorithmOptions(Timed: true);
            OutputOptions outputOptions = new OutputOptions(OutputType: OutputType.Console);

            using (var msg = new PageRankProxyMessageWriter(algorithmOptions, outputOptions, initalValue, epsilon, seperateLayers)) {
                MultiLayerProxy.MessagePassingExtension.PageRankProxy(Global.CloudStorage.ProxyList[0], msg);
            }  
        }

        private static void PageRankTopNodes (int numberOfTopNodes, bool seperateLayers) {
            AlgorithmOptions algorithmOptions = new AlgorithmOptions(Timed: true);
            OutputOptions outputOptions = new OutputOptions(OutputType: OutputType.Console);

            using (var msg = new PageRankTopNodesProxyMessageWriter(algorithmOptions, outputOptions, numberOfTopNodes, seperateLayers)) {
                MultiLayerProxy.MessagePassingExtension.PageRankTopNodesProxy(Global.CloudStorage.ProxyList[0], msg);
            }
        }

        private static void HITS (double initalValue, double epsilon, bool seperateLayers) {
            AlgorithmOptions algorithmOptions = new AlgorithmOptions(Timed: true);
            OutputOptions outputOptions = new OutputOptions(OutputType: OutputType.Console);

            using (var msg = new HITSProxyMessageWriter(algorithmOptions, outputOptions, initalValue, epsilon, seperateLayers)) {
                MultiLayerProxy.MessagePassingExtension.HITSProxy(Global.CloudStorage.ProxyList[0], msg);
            }  
        }


        private static void HITSTopAuthorities (int numberOfTopNodes, bool seperateLayers) {
            AlgorithmOptions algorithmOptions = new AlgorithmOptions(Timed: true);
            OutputOptions outputOptions = new OutputOptions(OutputType: OutputType.Console);

            using (var msg = new HITSTopNodesProxyMessageWriter(algorithmOptions, outputOptions, numberOfTopNodes, seperateLayers)) {
                MultiLayerProxy.MessagePassingExtension.HITSTopAuthoritiesProxy(Global.CloudStorage.ProxyList[0], msg);
            }  
        }


        private static void HITSTopHubs (int numberOfTopNodes, bool seperateLayers) {
            AlgorithmOptions algorithmOptions = new AlgorithmOptions(Timed: true);
            OutputOptions outputOptions = new OutputOptions(OutputType: OutputType.Console);

            using (var msg = new HITSTopNodesProxyMessageWriter(algorithmOptions, outputOptions, numberOfTopNodes, seperateLayers)) {
                MultiLayerProxy.MessagePassingExtension.HITSTopHubsProxy(Global.CloudStorage.ProxyList[0], msg);
            }  
        }
    }
}
