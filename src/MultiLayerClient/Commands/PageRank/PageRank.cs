using System;
using Trinity.Storage;

namespace MultiLayerClient.Commands {

  class PageRank: Command {

    private double InitalValue { get; set; }
    private double Epsilon { get; set; }
    private bool SeperateLayers { get; set; }

    public PageRank (RemoteStorage proxy): base (proxy) {
      Name = "Page Rank";
      Keyword = "pageRank";
      Description = "Runs Pagerank on all nodes.";
      Arguments = new string[] {"double", "double", "bool"};
      ArgumentsDescription = new string[] {"InitialValues", "Epsilon", "SeperateLayers"};
    }

    public override void ApplyArguments(string[] arguments) {
      InitalValue = double.Parse(arguments[0]);
      Epsilon = double.Parse(arguments[1]);
      SeperateLayers = bool.Parse(arguments[2]);
    }

    public override void Run() {
      AlgorithmOptions algorithmOptions = new AlgorithmOptions(Timed: true);
      OutputOptions outputOptions = new OutputOptions(OutputType: OutputType.Console);

      using (var msg = new PageRankProxyMessageWriter(algorithmOptions, outputOptions, InitalValue, Epsilon, SeperateLayers)) {
          MultiLayerProxy.MessagePassingExtension.PageRankProxy(Proxy, msg);
      }  
    }
  }
}
