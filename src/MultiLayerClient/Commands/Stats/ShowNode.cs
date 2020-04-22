using System;
using Trinity;
using Trinity.Storage;
using Trinity.Core.Lib;

namespace MultiLayerClient.Commands {


  class ShowNode: Command {

      private long Id { get; set; }

      private int Layer { get; set; }

      public ShowNode (RemoteStorage proxy): base (proxy) {
        Name = "Show Node";
        Keyword = "showNode";
        Arguments = new string[] {"long", "int"};
      }

    public override void ApplyArguments(string[] arguments) {
      Id = long.Parse(arguments[0]);
      Layer = int.Parse(arguments[1]);
    }

    public override void Run() {
      Node node = Global.CloudStorage.LoadNode(GetCellId(Id, Layer));

      Console.WriteLine("------");
      Console.WriteLine("Node {0} Layer {1}", Id, Layer);
      Console.WriteLine("PageRank: {0}", node.PageRankData.Value);
      Console.WriteLine("Authority: {0} | Hub: {1}", node.HITSData.AuthorityScore, node.HITSData.HubScore);
      Console.WriteLine("OutDegree: {0} | InDegree: {1} | TotalDegree: {2}", node.DegreeData.InDegree, node.DegreeData.OutDegree, node.DegreeData.TotalDegree);
      Console.WriteLine("------");
    }
    private long GetCellId (long id, int layer) {
        string nodeName = "n" + id + "l" + layer;
        return HashHelper.HashString2Int64(nodeName);
    }

  }

}
