using System;
using Trinity.Storage;

namespace MultiLayerClient.Commands {

  class PageRankTopNodes: Command {

    private int NumberOfTopNodes { get; set; }
    private bool SeperateLayers { get; set; }

    public PageRankTopNodes (RemoteStorage proxy): base (proxy) {
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
          MultiLayerProxy.MessagePassingExtension.PageRankTopNodesProxy(Proxy, msg);
      }
      Console.WriteLine("[Client] Finished PageRankTopNodes");
    }
  }
}
