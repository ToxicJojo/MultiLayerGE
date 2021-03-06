using MultiLayerLib;
using MultiLayerLib.Output;
using MultiLayerLib.MultiLayerProxy;

namespace MultiLayerClient.Commands {

  class EdgeCount: Command {

    public EdgeCount (Client client): base (client) {
      Name = "Edge Count";
      Keyword = "edgeCount";
      Description = "Counts the number of edges for each layer.";
      Arguments = new string[0];
      ArgumentsDescription = new string[0];
    }

    public override void ApplyArguments(string[] arguments) {}

    public override void Run() {
      using (var msg = new StandardAlgorithmMessageWriter(Client.AlgorithmOptions, Client.OutputOptions)) {
        using (var response = MessagePassingExtension.GetEdgeCountProxy(Client.Proxy, msg)) {
          OutputWriter.WriteOutput(response, Client.OutputOptions);
        }
      }      
    }
  }
}
