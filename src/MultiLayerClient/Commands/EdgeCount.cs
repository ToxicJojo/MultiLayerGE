using System;
using Trinity.Storage;

namespace MultiLayerClient.Commands {

  class EdgeCount: Command {

    public EdgeCount (RemoteStorage proxy): base (proxy) {
      Name = "Edge Count";
      Keyword = "edgeCount";
      Arguments = new string[0];
    }

    public override void ApplyArguments(string[] arguments) {}

    public override void Run() {
      AlgorithmOptions algorithmOptions = new AlgorithmOptions(Timed: true);
      OutputOptions outputOptions = new OutputOptions(OutputType: OutputType.CSV);

      Console.WriteLine("[Client] Started EdgeCount");
      using (var msg = new StandardAlgorithmMessageWriter(algorithmOptions, outputOptions)) {
          MultiLayerProxy.MessagePassingExtension.GetEdgeCountProxy(Proxy, msg);
      }      
      Console.WriteLine("[Client] Finished EdgeCount");
    }
  }
}
