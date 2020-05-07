using MultiLayerLib;
using MultiLayerLib.MultiLayerProxy;

namespace MultiLayerClient.Commands {

  class PageRankTopNodes: Command {

    private int NumberOfTopNodes { get; set; }
    private bool SeperateLayers { get; set; }

    public PageRankTopNodes (Client client): base (client) {
      Name = "Pagerank top nodes";
      Keyword = "pageRankTopNodes";
      Description = "Finds the nodes with the highest pagerank values.";
      Arguments = new string[] {"int", "bool"};
      Arguments = new string[] {"NumerOfTopNodes", "SeperateLayers"};
    }

    public override void ApplyArguments(string[] arguments) {
      NumberOfTopNodes = int.Parse(arguments[0]);
      SeperateLayers = bool.Parse(arguments[1]);
    }

    public override void Run() {
      using (var msg = new PageRankTopNodesProxyMessageWriter(Client.AlgorithmOptions, Client.OutputOptions, NumberOfTopNodes, SeperateLayers)) {
          MessagePassingExtension.PageRankTopNodesProxy(Client.Proxy, msg);
      }
    }
  }
}
