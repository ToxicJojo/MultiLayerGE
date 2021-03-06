using MultiLayerLib;
using MultiLayerLib.Output;
using MultiLayerLib.MultiLayerProxy;

namespace MultiLayerClient.Commands {

  class GraphDensity: Command {

    public GraphDensity (Client client): base (client) {
      Name = "Graph Density";
      Keyword = "graphDensity";
      Arguments = new string[0];
      ArgumentsDescription = new string[0];
    }

    public override void ApplyArguments(string[] arguments) {}

    public override void Run() {
      using (var msg = new StandardAlgorithmMessageWriter(Client.AlgorithmOptions, Client.OutputOptions)) {
          using (var response = MessagePassingExtension.GetGraphDensityProxy(Client.Proxy, msg)) {
            OutputWriter.WriteOutput(response, Client.OutputOptions);
          }
      }      
    }
  }
}
