using System;
using Trinity;

namespace MultiLayerClient.Commands {

  class GraphDensity: Command {

    public GraphDensity () {
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
          MultiLayerProxy.MessagePassingExtension.GetGraphDensityProxy(Global.CloudStorage.ProxyList[0], msg);
      }      
      Console.WriteLine("[Client] Finished GraphDensity");
    }
  }
}
