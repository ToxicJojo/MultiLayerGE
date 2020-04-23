using System;
using Trinity.Storage;

namespace MultiLayerClient.Commands {

  class NodeCount: Command {

    public NodeCount (RemoteStorage proxy): base (proxy) {
      Name = "Node Count";
      Keyword = "nodeCount";
      Description = "Executes a Node count";
      Arguments = new string[0];
      ArgumentsDescription = new string[0];
    }

    public override void ApplyArguments(string[] arguments) {}

    public override void Run() {
      AlgorithmOptions algorithmOptions = new AlgorithmOptions(Timed: true);
      OutputOptions outputOptions = new OutputOptions(OutputType: OutputType.CSV);

      using (var msg = new StandardAlgorithmMessageWriter(algorithmOptions, outputOptions)) {
          MultiLayerProxy.MessagePassingExtension.GetNodeCountProxy(Proxy, msg);
      }      
    }
  }
}
