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
      Arguments = new string[] {"double", "double", "bool"};
    }

    public override void ApplyArguments(string[] arguments) {
      InitalValue = double.Parse(arguments[0]);
      Epsilon = double.Parse(arguments[1]);
      SeperateLayers = bool.Parse(arguments[2]);
    }

    public override void Run() {
      Console.WriteLine("[Client] Started PageRank");
      AlgorithmOptions algorithmOptions = new AlgorithmOptions(Timed: true);
      OutputOptions outputOptions = new OutputOptions(OutputType: OutputType.Console);

      using (var msg = new PageRankProxyMessageWriter(algorithmOptions, outputOptions, InitalValue, Epsilon, SeperateLayers)) {
          MultiLayerProxy.MessagePassingExtension.PageRankProxy(Proxy, msg);
      }  
      Console.WriteLine("[Client] Finished PageRank");
    }
  }
}
