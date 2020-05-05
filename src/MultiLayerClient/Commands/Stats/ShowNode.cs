using System;
using MultiLayerLib;

namespace MultiLayerClient.Commands {
  class ShowNode: Command {

      private long Id { get; set; }

      private int Layer { get; set; }

      public ShowNode (): base (null) {
        Name = "Show Node";
        Keyword = "showNode";
        Description = "Shows information about a single node";
        Arguments = new string[] {"long", "int"};
        ArgumentsDescription = new string[] {"Id", "Layer"};
      }

    public override void ApplyArguments(string[] arguments) {
      Id = long.Parse(arguments[0]);
      Layer = int.Parse(arguments[1]);
    }

    public override void Run() {
      Node node = Graph.LoadNode(Id, Layer);

      Console.WriteLine("------");
      Console.WriteLine("Node {0} Layer {1}", Id, Layer);
      Console.WriteLine("PageRank: {0}", node.PageRankData.Value);
      Console.WriteLine("Authority: {0} | Hub: {1}", node.HITSData.AuthorityScore, node.HITSData.HubScore);
      Console.WriteLine("OutDegree: {0} | InDegree: {1} | TotalDegree: {2}", node.DegreeData.InDegree, node.DegreeData.OutDegree, node.DegreeData.TotalDegree);
      Console.WriteLine("------");
    }
  }

}
