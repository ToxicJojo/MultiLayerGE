using System;
using System.Collections.Generic;
using System.Linq;
using Trinity;
using MultiLayerProxy.Proxy;
using MultiLayerProxy.Output;
using MultiLayerLib;
using MultiLayerLib.MultiLayerServer;

namespace MultiLayerProxy.Algorithms {

  class HITSTopHubs: Algorithm {


    public int NumerOfTopNodes { get; set; }

    private bool SeperateLayers { get; set; }

    public HITSTopHubs (MultiLayerProxyImpl proxy, int numberOfTopNodes, bool seperateLayers): base(proxy) {
      NumerOfTopNodes = numberOfTopNodes;
      SeperateLayers = seperateLayers;
    }


    public override void Run() {
      foreach(var server in Global.CloudStorage) {
        using (var msg = new HITSTopNodesServerMessageWriter(NumerOfTopNodes, SeperateLayers)) {
          MessagePassingExtension.HITSTopHubsServer(server, msg);
        }
      }

      List<List<long>> phaseResults = Proxy.WaitForPhaseResultsAsLong(Phases.HITSTopHubs);

      List<long> topNodeIds = new List<long>();
      foreach(List<long> localTopNodes in phaseResults) {
        topNodeIds.AddRange(localTopNodes);
      }
      
      List<Node> topNodes = new List<Node>();

      topNodes = topNodeIds.Select(nodeId => {
        Node node = Global.CloudStorage.LoadNode(nodeId);
        return node;
      }).ToList();

      List<List<string>> resultTable = new List<List<string>>();

      if (SeperateLayers) {
        var topN = topNodeIds.Select(nodeId => {
          Node node = Global.CloudStorage.LoadNode(nodeId);
          return node;
        }).GroupBy(node => node.Layer).Select(group => new { Layer = group.Key, Nodes = group.OrderByDescending(node => node.HITSData.HubScore).Take(NumerOfTopNodes) }).OrderBy(group => group.Layer);

        foreach (var group in topN) {
          foreach(Node node in group.Nodes) {
            List<string> resultRow = new List<string>();
            resultRow.Add(node.Id.ToString());
            resultRow.Add(node.Layer.ToString());
            resultRow.Add(node.HITSData.HubScore.ToString());

            resultTable.Add(resultRow);       
          }
        }
      } else {
        foreach(Node node in topNodes) {
          List<string> resultRow = new List<string>();
          resultRow.Add(node.Id.ToString());
          resultRow.Add(node.Layer.ToString());
          resultRow.Add(node.HITSData.HubScore.ToString());

          resultTable.Add(resultRow);
        }
      }

      AlgorithmResult result = new AlgorithmResult("HITSTopHubs", resultTable);
      this.Result = result;
    }
  }

}
