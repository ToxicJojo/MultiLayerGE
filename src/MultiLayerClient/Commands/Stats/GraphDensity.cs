using System;
using Trinity.Storage;

namespace MultiLayerClient.Commands {

  class GraphDensity: Command {

    public GraphDensity (RemoteStorage proxy): base (proxy) {
      Name = "Graph Density";
      Keyword = "graphDensity";
      Arguments = new string[0];
    }

    public override void ApplyArguments(string[] arguments) {}

    public override void Run() {
      AlgorithmOptions algorithmOptions = new AlgorithmOptions(Timed: true);
      OutputOptions outputOptions = new OutputOptions(OutputType: OutputType.CSV);

      Console.WriteLine("[Client] Started GraphDensity");
      using (var msg = new StandardAlgorithmMessageWriter(algorithmOptions, outputOptions)) {
          MultiLayerProxy.MessagePassingExtension.GetGraphDensityProxy(Proxy, msg);
      }      
      Console.WriteLine("[Client] Finished GraphDensity");
    }
  }
}
