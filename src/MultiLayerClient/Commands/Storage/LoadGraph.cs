using System;
using Trinity.Storage;

namespace MultiLayerClient.Commands {

  class LoadGraph: Command {

    private string ConfigFilePath { get; set; }

    public LoadGraph (RemoteStorage proxy): base (proxy) {
      Name = "Load graph";
      Keyword = "loadGraph";
      Description = "Loads a graph from the given config file.";
      Arguments = new string[] { "string" };
      ArgumentsDescription = new string[] { "configFilePath" };
    }

    public override void ApplyArguments(string[] arguments) {
      ConfigFilePath = arguments[0];
    }

    public override void Run() {
      AlgorithmOptions algorithmOptions = new AlgorithmOptions(Timed:true);
      OutputOptions outputOptions = new OutputOptions(OutputType: OutputType.CSV);
      using (var msg = new LoadGraphProxyMessageWriter(algorithmOptions, outputOptions, ConfigFilePath)) {
          MultiLayerProxy.MessagePassingExtension.LoadGraphProxy(Proxy, msg);
      }   
    }
  }
}
