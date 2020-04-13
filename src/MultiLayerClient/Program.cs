using System;
using Trinity;
using Trinity.Core.Lib;

namespace MultiLayerClient {
    class Program {
        static void Main(string[] args) {
            // Trinity doesn't load the config file correctly if we don't tell it to.
            TrinityConfig.LoadConfig();
            TrinityConfig.CurrentRunningMode = RunningMode.Client;


            LoadGraph("/home/thiel/MultiLayerGE/data/journals/journals_config.txt");
            //LoadGraph("/home/thiel/MultiLayerGE/data/multiplex6/multiplex6_config.txt");

            //Global.CloudStorage.SaveStorage();

            /*
            Console.WriteLine("Loading from ge storage.");
            Global.CloudStorage.LoadStorage();
            Console.WriteLine("Finished Loading from ge storage.");
            */


            //GetNodeCount();
            //GetEdgeCount();
            //GetGraphDensity();
            GetDegree(true);
            //PageRank(1, 1, true);
            //PageRankTopNodes(5, true);
            //HITS(1, 2000, true);
            /*
            HITSTopAuthorities(1,true);
            HITSTopHubs(5,true);
            PageRank(1, 0.35, true);
            */
            //ShowNode(2, 1);
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

        public static void GetGraphDensity () {
            AlgorithmOptions algorithmOptions = new AlgorithmOptions(Timed: true);
            OutputOptions outputOptions = new OutputOptions(OutputType: OutputType.CSV);

            using (var msg = new StandardAlgorithmMessageWriter(algorithmOptions, outputOptions)) {
                MultiLayerProxy.MessagePassingExtension.GetGraphDensityProxy(Global.CloudStorage.ProxyList[0], msg);
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
            OutputOptions outputOptions = new OutputOptions(OutputType: OutputType.CSV);

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

        public static void GetDegree (bool seperateLayers) {
            AlgorithmOptions algorithmOptions = new AlgorithmOptions(Timed: true);
            OutputOptions outputOptions = new OutputOptions(OutputType: OutputType.CSV);

            using (var msg = new DegreeProxyMessageWriter(algorithmOptions, outputOptions, seperateLayers)) {
                MultiLayerProxy.MessagePassingExtension.DegreeProxy(Global.CloudStorage.ProxyList[0], msg);
            }            
        }

        public static long GetCellId (long id, int layer) {
            string nodeName = "n" + id + "l" + layer;
            return HashHelper.HashString2Int64(nodeName);
        }

        private static void ShowNode(int id, int layer) {
            Node node = Global.CloudStorage.LoadNode(GetCellId(id, layer));

            Console.WriteLine("------");
            Console.WriteLine("Node {0} Layer {1}", id, layer);
            Console.WriteLine("PageRank: {0}", node.PageRankData.Value);
            Console.WriteLine("Authority: {0} | Hub: {1}", node.HITSData.AuthorityScore, node.HITSData.HubScore);
            Console.WriteLine("OutDegree: {0} | InDegree: {1} | TotalDegree: {2}", node.DegreeData.InDegree, node.DegreeData.OutDegree, node.DegreeData.TotalDegree);
            Console.WriteLine("------");
        }
    }
}
