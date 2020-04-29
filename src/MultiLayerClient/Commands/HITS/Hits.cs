namespace MultiLayerClient.Commands {

  class HITS: Command {

    private double InitalValue { get; set; }
    private double Epsilon { get; set; }
    private bool SeperateLayers { get; set; }

    public HITS (Client client): base (client) {
      Name = "HITS";
      Keyword = "hits";
      Description = "Runs HITS on all nodes.";
      Arguments = new string[] {"double", "double", "bool"};
      ArgumentsDescription = new string[] {"InitialValues", "Epsilon", "SeperateLayers"};
    }

    public override void ApplyArguments(string[] arguments) {
      InitalValue = double.Parse(arguments[0]);
      Epsilon = double.Parse(arguments[1]);
      SeperateLayers = bool.Parse(arguments[2]);
    }

    public override void Run() {
      using (var msg = new HITSProxyMessageWriter(Client.AlgorithmOptions, Client.OutputOptions, InitalValue, Epsilon, SeperateLayers)) {
          MultiLayerProxy.MessagePassingExtension.HITSProxy(Client.Proxy, msg);
      }  
    }
  }
}
