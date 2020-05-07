using MultiLayerLib;
using MultiLayerLib.MultiLayerProxy;

namespace MultiLayerClient.Commands {

  class EgoNetwork: Command {
    private long Id { get; set; }
    private int Layer { get; set; }
    private bool SeperateLayers { get; set; }

    public EgoNetwork (Client client): base (client) {
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
      using (var msg = new EgoNetworkMessageProxyWriter(Client.AlgorithmOptions, Client.OutputOptions, Id, Layer, SeperateLayers)) {
          MessagePassingExtension.EgoNetworkProxy(Client.Proxy, msg);
      }          
    }
  }
}
