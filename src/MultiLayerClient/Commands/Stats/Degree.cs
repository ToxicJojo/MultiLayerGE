using System;
using Trinity.Storage;

namespace MultiLayerClient.Commands {

  class Degree: Command {

    private bool SeperateLayers { get; set; }

    public Degree (RemoteStorage proxy): base (proxy) {
      Name = "Degree";
      Keyword = "degree";
      Description = "Calculates the in/out/total degree for all nodes.";
      Arguments = new string[] { "bool" };
      ArgumentsDescription = new string[] { "SeperateLayers" };
    }

    public override void ApplyArguments(string[] arguments) {
      SeperateLayers = bool.Parse(arguments[0]);
    }

    public override void Run() {
      AlgorithmOptions algorithmOptions = new AlgorithmOptions(Timed: true);
      OutputOptions outputOptions = new OutputOptions(OutputType: OutputType.CSV);

      using (var msg = new DegreeProxyMessageWriter(algorithmOptions, outputOptions, SeperateLayers)) {
          MultiLayerProxy.MessagePassingExtension.DegreeProxy(Proxy, msg);
      }        
    }
  }
}
