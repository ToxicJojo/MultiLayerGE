using MultiLayerLib;
using MultiLayerLib.MultiLayerProxy;

namespace MultiLayerClient.Commands {

  class HITSTopHubs: Command {

    private int NumberOfTopNodes { get; set; }
    private bool SeperateLayers { get; set; }

    public HITSTopHubs (Client client): base (client) {
      Name = "Hits top hubs";
      Keyword = "hitsTopHubs";
      Description = "Finds the nodes with the highest hub score.";
      Arguments = new string[] {"int", "bool"};
      Arguments = new string[] {"NumberOfTopNodes", "SeperateLayers"};
    }

    public override void ApplyArguments(string[] arguments) {
      NumberOfTopNodes = int.Parse(arguments[0]);
      SeperateLayers = bool.Parse(arguments[1]);
    }

    public override void Run() {
      using (var msg = new HITSTopNodesProxyMessageWriter(Client.AlgorithmOptions, Client.OutputOptions, NumberOfTopNodes, SeperateLayers)) {
          MessagePassingExtension.HITSTopHubsProxy(Client.Proxy, msg);
      }
    }
  }
}
