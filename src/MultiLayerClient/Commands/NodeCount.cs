using System;
using Trinity;

namespace MultiLayerClient.Commands {

  class NodeCount: Command {

    public NodeCount () {
      Name = "Node Count";
      Keyword = "nodeCount";
      Arguments = new string[0];
    }

    public override void ApplyArguments(string[] arguments) {}

    public override void Run() {
      AlgorithmOptions algorithmOptions = new AlgorithmOptions(Timed: true);
      OutputOptions outputOptions = new OutputOptions(OutputType: OutputType.CSV);

      Console.WriteLine("[Client] Started NodeCount");
      using (var msg = new StandardAlgorithmMessageWriter(algorithmOptions, outputOptions)) {
          MultiLayerProxy.MessagePassingExtension.GetNodeCountProxy(Global.CloudStorage.ProxyList[0], msg);
      }      
      Console.WriteLine("[Client] Finished NodeCount");
    }
  }
}
