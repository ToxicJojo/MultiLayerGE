using System;
using Trinity;

namespace MultiLayerClient.Commands {

  class PageRankTopNodes: Command {

    private int NumberOfTopNodes { get; set; }
    private bool SeperateLayers { get; set; }

    public PageRankTopNodes () {
      Name = "Pagerank top nodes";
      Keyword = "pageRankTopNodes";
      Arguments = new string[] {"int", "bool"};
    }

    public override void ApplyArguments(string[] arguments) {
      NumberOfTopNodes = int.Parse(arguments[0]);
      SeperateLayers = bool.Parse(arguments[1]);
    }

    public override void Run() {
      Console.WriteLine("[Client] Started PageRankTopNodes");
      AlgorithmOptions algorithmOptions = new AlgorithmOptions(Timed: true);
      OutputOptions outputOptions = new OutputOptions(OutputType: OutputType.CSV);

      using (var msg = new PageRankTopNodesProxyMessageWriter(algorithmOptions, outputOptions, NumberOfTopNodes, SeperateLayers)) {
          MultiLayerProxy.MessagePassingExtension.PageRankTopNodesProxy(Global.CloudStorage.ProxyList[0], msg);
      }
      Console.WriteLine("[Client] Finished PageRankTopNodes");
    }
  }
}
