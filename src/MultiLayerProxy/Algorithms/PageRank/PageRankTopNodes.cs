using System;
using System.Collections.Generic;
using System.Linq;
using Trinity;
using MultiLayerProxy.Proxy;
using MultiLayerLib;
using MultiLayerLib.MultiLayerServer;

namespace MultiLayerProxy.Algorithms {

  class PageRankTopNodes: Algorithm {


    public int NumerOfTopNodes { get; set; }

    private bool SeperateLayers { get; set; }

    private List<Node> topNodes;

    List<long> topNodeIds;

    public PageRankTopNodes (MultiLayerProxyImpl proxy, int numberOfTopNodes, bool seperateLayers): base(proxy) {
      NumerOfTopNodes = numberOfTopNodes;
      SeperateLayers = seperateLayers;
      this.Name = "PageRankTopNodes";
    }


    public override void Run() {
      foreach(var server in Global.CloudStorage) {
        using (var msg = new PageRankTopNodesServerMessageWriter(NumerOfTopNodes, SeperateLayers)) {
          MessagePassingExtension.PageRankTopNodesServer(server, msg);
        }
      }

      List<List<long>> phaseResults = Proxy.WaitForPhaseResultsAsLong(Phases.PageRankTopNodes);

      topNodeIds = new List<long>();
      foreach(List<long> localTopNodes in phaseResults) {
        topNodeIds.AddRange(localTopNodes);
      }
      
      topNodes = new List<Node>();

      topNodes = topNodeIds.Select(nodeId => {
        Node node = Global.CloudStorage.LoadNode(nodeId);
        return node;
      }).ToList();//.OrderByDescending(node => node.PageRankData.Value).Take(this.NumerOfTopNodes).ToList();

      
    }

    public override List<List<string>> GetResultTable(OutputOptions options) {
      List<List<string>> resultTable = new List<List<string>>();

      if (SeperateLayers) {
        var topN = topNodeIds.Select(nodeId => {
          Node node = Global.CloudStorage.LoadNode(nodeId);
          return node;
        }).GroupBy(node => node.Layer).Select(group => new { Layer = group.Key, Nodes = group.OrderByDescending(node => node.PageRankData.Value).Take(NumerOfTopNodes) }).OrderBy(group => group.Layer);

        foreach (var group in topN) {
          foreach(Node node in group.Nodes) {
            List<string> resultRow = new List<string>();
            resultRow.Add(node.Id.ToString());
            resultRow.Add(node.Layer.ToString());
            resultRow.Add(node.PageRankData.Value.ToString());

            resultTable.Add(resultRow);       
          }
        }
      } else {
        foreach(Node node in topNodes.OrderByDescending(node => node.PageRankData.Value).Take(NumerOfTopNodes).ToList()) {
          List<string> resultRow = new List<string>();
          resultRow.Add(node.Id.ToString());
          resultRow.Add(node.Layer.ToString());
          resultRow.Add(node.PageRankData.Value.ToString());

          resultTable.Add(resultRow);
        }
      }

      return resultTable;
    }
  }

}
