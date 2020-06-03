using MultiLayerLib;
using MultiLayerLib.Output;
using MultiLayerLib.MultiLayerProxy;

namespace MultiLayerClient.Commands {

  class NodeCount: Command {

    public NodeCount (Client client): base (client) {
      Name = "Node Count";
      Keyword = "nodeCount";
      Description = "Executes a Node count";
      Arguments = new string[0];
      ArgumentsDescription = new string[0];
    }

    public override void ApplyArguments(string[] arguments) {}

    public override void Run() {
      using (var msg = new StandardAlgorithmMessageWriter(Client.AlgorithmOptions, Client.OutputOptions)) {
        using(AlgorithmResultReader response = MessagePassingExtension.GetNodeCountProxy(Client.Proxy, msg)) {
          OutputWriter.WriteOutput(response, Client.OutputOptions);
        }
      }
    }
  }
}
