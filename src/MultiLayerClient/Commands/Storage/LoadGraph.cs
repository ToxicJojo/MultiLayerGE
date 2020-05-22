using MultiLayerLib;
using MultiLayerLib.MultiLayerProxy;

namespace MultiLayerClient.Commands {

  class LoadGraph: Command {

    private string ConfigFilePath { get; set; }

    private EdgeType EdgeType { get; set; }

    public LoadGraph (Client client): base (client) {
      Name = "Load graph";
      Keyword = "loadGraph";
      Description = "Loads a graph from the given config file.";
      Arguments = new string[] { "string", "string" };
      ArgumentsDescription = new string[] { "configFilePath", "edgeLoader" };
    }

    public override void ApplyArguments(string[] arguments) {
      ConfigFilePath = arguments[0];

      switch(arguments[1]) {
        case "MultilayerDirectedEdge":
          EdgeType = EdgeType.MultilayerDirectedEdge;
          break;
        case "MultiLayerDirectedWeightedEdge":
          EdgeType = EdgeType.MultiLayerDirectedWeightedEdge;
          break;
        case "Journals":
          EdgeType = EdgeType.Journals;
          break;
      }
    }

    public override void Run() {
      AlgorithmOptions algorithmOptions = new AlgorithmOptions(Timed:true);
      OutputOptions outputOptions = new OutputOptions(OutputType: OutputType.CSV);
      using (var msg = new LoadGraphProxyMessageWriter(algorithmOptions, outputOptions, ConfigFilePath, this.EdgeType)) {
          MessagePassingExtension.LoadGraphProxy(Client.Proxy, msg);
      }   
    }
  }
}
