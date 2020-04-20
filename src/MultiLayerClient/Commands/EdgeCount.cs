using System;
using Trinity;

namespace MultiLayerClient.Commands {

  class EdgeCount: Command {

    public EdgeCount () {
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
          MultiLayerProxy.MessagePassingExtension.GetEdgeCountProxy(Global.CloudStorage.ProxyList[0], msg);
      }      
      Console.WriteLine("[Client] Finished EdgeCount");
    }
  }
}
