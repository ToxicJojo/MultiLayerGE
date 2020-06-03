using System;
using MultiLayerLib;
using MultiLayerLib.Output;
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
      if (!Graph.HasNode(Id, Layer)) {
        Console.WriteLine("No node found with Id: {0} in Layer: {1}", Id, Layer);
        return;
      }

      using (var msg = new EgoNetworkMessageProxyWriter(Client.AlgorithmOptions, Client.OutputOptions, Id, Layer, SeperateLayers)) {
        using (AlgorithmResultReader response = MessagePassingExtension.EgoNetworkProxy(Client.Proxy, msg)) {
          OutputWriter.WriteOutput(response, Client.OutputOptions);
        }
      }  
    }
  }
}
