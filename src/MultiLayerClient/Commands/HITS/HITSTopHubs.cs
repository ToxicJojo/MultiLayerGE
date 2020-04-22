using System;
using Trinity.Storage;

namespace MultiLayerClient.Commands {

  class HITSTopHubs: Command {

    private int NumberOfTopNodes { get; set; }
    private bool SeperateLayers { get; set; }

    public HITSTopHubs (RemoteStorage proxy): base (proxy) {
      Name = "Hits top hubs";
      Keyword = "hitsTopHubs";
      Arguments = new string[] {"int", "bool"};
    }

    public override void ApplyArguments(string[] arguments) {
      NumberOfTopNodes = int.Parse(arguments[0]);
      SeperateLayers = bool.Parse(arguments[1]);
    }

    public override void Run() {
      Console.WriteLine("[Client] Started Hits top hubs");
      AlgorithmOptions algorithmOptions = new AlgorithmOptions(Timed: true);
      OutputOptions outputOptions = new OutputOptions(OutputType: OutputType.CSV);

      using (var msg = new HITSTopNodesProxyMessageWriter(algorithmOptions, outputOptions, NumberOfTopNodes, SeperateLayers)) {
          MultiLayerProxy.MessagePassingExtension.HITSTopHubsProxy(Proxy, msg);
      }
      Console.WriteLine("[Client] Finished Hits top hubs");
    }
  }
}
