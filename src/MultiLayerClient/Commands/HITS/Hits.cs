using System;
using Trinity.Storage;

namespace MultiLayerClient.Commands {

  class HITS: Command {

    private double InitalValue { get; set; }
    private double Epsilon { get; set; }
    private bool SeperateLayers { get; set; }

    public HITS (RemoteStorage proxy): base (proxy) {
      Name = "HITS";
      Keyword = "hits";
      Arguments = new string[] {"double", "double", "bool"};
    }

    public override void ApplyArguments(string[] arguments) {
      InitalValue = double.Parse(arguments[0]);
      Epsilon = double.Parse(arguments[1]);
      SeperateLayers = bool.Parse(arguments[2]);
    }

    public override void Run() {
      Console.WriteLine("[Client] Started HITS");
      AlgorithmOptions algorithmOptions = new AlgorithmOptions(Timed: true);
      OutputOptions outputOptions = new OutputOptions(OutputType: OutputType.CSV);

      using (var msg = new HITSProxyMessageWriter(algorithmOptions, outputOptions, InitalValue, Epsilon, SeperateLayers)) {
          MultiLayerProxy.MessagePassingExtension.HITSProxy(Proxy, msg);
      }  
      Console.WriteLine("[Client] Finished HITS");
    }
  }
}
