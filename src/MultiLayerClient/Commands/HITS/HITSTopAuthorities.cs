using System;
using Trinity.Storage;

namespace MultiLayerClient.Commands {

  class HITSTopAuthorities: Command {

    private int NumberOfTopNodes { get; set; }
    private bool SeperateLayers { get; set; }

    public HITSTopAuthorities (RemoteStorage proxy): base (proxy) {
      Name = "Hits top authorities";
      Keyword = "hitsTopAuthorities";
      Arguments = new string[] {"int", "bool"};
    }

    public override void ApplyArguments(string[] arguments) {
      NumberOfTopNodes = int.Parse(arguments[0]);
      SeperateLayers = bool.Parse(arguments[1]);
    }

    public override void Run() {
      Console.WriteLine("[Client] Started Hits top authorities");
      AlgorithmOptions algorithmOptions = new AlgorithmOptions(Timed: true);
      OutputOptions outputOptions = new OutputOptions(OutputType: OutputType.CSV);

      using (var msg = new HITSTopNodesProxyMessageWriter(algorithmOptions, outputOptions, NumberOfTopNodes, SeperateLayers)) {
          MultiLayerProxy.MessagePassingExtension.HITSTopAuthoritiesProxy(Proxy, msg);
      }
      Console.WriteLine("[Client] Finished Hits top authorities");
    }
  }
}
