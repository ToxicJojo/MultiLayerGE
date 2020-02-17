using System;
using System.Collections.Generic;
using System.Linq;
using Trinity;
using MultiLayerProxy.Proxy;
using MultiLayerProxy.Output;

namespace MultiLayerProxy.Algorithms {

  class PageRankTopNodes: Algorithm {


    public int NumerOfTopNodes { get; set; }

    public PageRankTopNodes (MultiLayerProxyImpl proxy, int numberOfTopNodes): base(proxy) {
      NumerOfTopNodes = numberOfTopNodes;
    }


    public override void Run() {
      foreach(var server in Global.CloudStorage) {
        using (var msg = new PageRankTopNodesServerMessageWriter(NumerOfTopNodes)) {
          MultiLayerServer.MessagePassingExtension.PageRankTopNodesServer(server, msg);
        }
      }

      List<List<long>> phaseResults = Proxy.WaitForPhaseResultsAsLong(Phases.PageRankTopNodes);

      List<long> topNodeIds = new List<long>();
      foreach(List<long> localTopNodes in phaseResults) {
        topNodeIds.AddRange(localTopNodes);
      }
      
      List<Node> topNodes = new List<Node>();

      topNodes = topNodeIds.Select(nodeId => {
        Node node = Global.CloudStorage.LoadNode(nodeId);
        return node;
      }).OrderByDescending(node => node.PageRankData.Value).Take(this.NumerOfTopNodes).ToList();

      List<List<string>> resultTable = new List<List<string>>();

      foreach(Node node in topNodes) {
        List<string> resultRow = new List<string>();
        resultRow.Add(node.Id.ToString());
        resultRow.Add(node.Layer.ToString());
        resultRow.Add(node.PageRankData.Value.ToString());

        resultTable.Add(resultRow);
      }

      AlgorithmResult result = new AlgorithmResult("PageRankTopNodes", resultTable);
      this.Result = result;
    }
  }

}
