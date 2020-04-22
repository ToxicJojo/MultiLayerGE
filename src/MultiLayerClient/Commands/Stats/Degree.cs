using System;
using Trinity.Storage;

namespace MultiLayerClient.Commands {

  class Degree: Command {

    private bool SeperateLayers { get; set; }

    public Degree (RemoteStorage proxy): base (proxy) {
      Name = "Degree";
      Keyword = "degree";
      Arguments = new string[] { "bool" };
    }

    public override void ApplyArguments(string[] arguments) {
      SeperateLayers = bool.Parse(arguments[0]);
    }

    public override void Run() {
      Console.WriteLine("[Client] Started Degree");
      AlgorithmOptions algorithmOptions = new AlgorithmOptions(Timed: true);
      OutputOptions outputOptions = new OutputOptions(OutputType: OutputType.CSV);

      using (var msg = new DegreeProxyMessageWriter(algorithmOptions, outputOptions, SeperateLayers)) {
          MultiLayerProxy.MessagePassingExtension.DegreeProxy(Proxy, msg);
      }        
      Console.WriteLine("[Client] Finished Degree");
    }
  }
}
