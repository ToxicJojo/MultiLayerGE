using System;
using Trinity.Storage;

namespace MultiLayerClient.Commands {

  class EgoNetwork: Command {
    private long Id { get; set; }
    private int Layer { get; set; }
    private bool SeperateLayers { get; set; }

    public EgoNetwork (RemoteStorage proxy): base (proxy) {
      Name = "EgoNetwork";
      Keyword = "egoNetwork";
      Description = "Finds the ego network for a node.";
      Arguments = new string[] { "long", "int", "bool" };
      ArgumentsDescription = new string[] { "Id", "Layer", "SeperateLayers" };
    }

    public override void ApplyArguments(string[] arguments) {
      Id = long.Parse(arguments[0]);
      Layer = int.Parse(arguments[1]);
      SeperateLayers = bool.Parse(arguments[2]);
    }

    public override void Run() {
      AlgorithmOptions algorithmOptions = new AlgorithmOptions(Timed: true);
      OutputOptions outputOptions = new OutputOptions(OutputType: OutputType.CSV);

      using (var msg = new EgoNetworkMessageProxyWriter(algorithmOptions, outputOptions, Id, Layer, SeperateLayers)) {
          MultiLayerProxy.MessagePassingExtension.EgoNetworkProxy(Proxy, msg);
      }          
    }
  }
}
