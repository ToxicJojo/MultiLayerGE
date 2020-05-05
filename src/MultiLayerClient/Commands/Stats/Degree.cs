using MultiLayerLib;
using MultiLayerLib.MultiLayerProxy;

namespace MultiLayerClient.Commands {

  class Degree: Command {

    private bool SeperateLayers { get; set; }

    public Degree (Client client): base (client) {
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
      using (var msg = new DegreeProxyMessageWriter(Client.AlgorithmOptions, Client.OutputOptions, SeperateLayers)) {
          MessagePassingExtension.DegreeProxy(Client.Proxy, msg);
      }        
    }
  }
}
