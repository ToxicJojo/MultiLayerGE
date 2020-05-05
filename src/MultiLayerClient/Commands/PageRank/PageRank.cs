using MultiLayerLib;
using MultiLayerLib.MultiLayerProxy;

namespace MultiLayerClient.Commands {

  class PageRank: Command {

    private double InitalValue { get; set; }
    private double Epsilon { get; set; }
    private bool SeperateLayers { get; set; }

    public PageRank (Client client): base (client) {
      Name = "Page Rank";
      Keyword = "pageRank";
      Description = "Runs Pagerank on all nodes.";
      Arguments = new string[] {"double", "double", "bool"};
      ArgumentsDescription = new string[] {"InitialValues", "Epsilon", "SeperateLayers"};
    }

    public override void ApplyArguments(string[] arguments) {
      InitalValue = double.Parse(arguments[0]);
      Epsilon = double.Parse(arguments[1]);
      SeperateLayers = bool.Parse(arguments[2]);
    }

    public override void Run() {
      using (var msg = new PageRankProxyMessageWriter(Client.AlgorithmOptions, Client.OutputOptions, InitalValue, Epsilon, SeperateLayers)) {
          MessagePassingExtension.PageRankProxy(Client.Proxy, msg);
      }
    }
  }
}
