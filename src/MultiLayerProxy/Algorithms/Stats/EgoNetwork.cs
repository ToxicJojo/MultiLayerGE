using System.Collections.Generic;
using Trinity;
using System;
using MultiLayerProxy.Proxy;
using MultiLayerProxy.Output;
using Trinity.TSL.Lib;
using Trinity.Core.Lib;
using MultiLayerProxy.Util;
using MultiLayerLib;
using MultiLayerLib.MultiLayerServer;

namespace MultiLayerProxy.Algorithms {
  /// <summary>
  /// Counts all the edges in the graph. The edges are counted for each layer seperatly.
  /// </summary>
  class EgoNetwork: Algorithm {

    private bool SeperateLayers { get; set; }
    
    private long Id { get; set; }

    private int Layer { get; set; }


    public EgoNetwork (MultiLayerProxyImpl proxy, long id, int layer, bool seperateLayers): base(proxy) {
      SeperateLayers = seperateLayers;
      Id = id;
      Layer = layer;
    }

    public override void Run() {
      
      List<Node> outgoingNetwork = GetEgoNetwork(Id, Layer, SeperateLayers);

      foreach(var server in Global.CloudStorage) {
        using (var msg = new EgoNetworkMessageServerWriter(Id, Layer, SeperateLayers)) {
          MessagePassingExtension.EgoNetworkServer(server, msg);
        }
      }

      List<List<long>> phaseResults = Proxy.WaitForPhaseResultsAsLong(Phases.EgoNetwork);
      List<long> incomingIds = new List<long>();
      foreach(List<long> result in phaseResults) {
        incomingIds.AddRange(result);
      }

      List<Node> incomingNetwork = new List<Node>();
      foreach(long id in incomingIds) {
        incomingNetwork.Add(Global.CloudStorage.LoadNode(id));
      }

      WriteOutput(outgoingNetwork, incomingNetwork);
   }

    private void WriteOutput(List<Node> outgoingNetwork, List<Node> incomingNetwork) {
      List<List<String>> output = new List<List<string>>();

      output.Add(new List<string> {"Outgoing: "});
      PrintEgoNetwork(outgoingNetwork, output);
      output.Add(new List<string> {"Incoming: "});
      PrintEgoNetwork(incomingNetwork, output);

      Result = new AlgorithmResult("EgoNetwork" + Layer + "." + Id, output);
    }
    

    private List<Node> GetEgoNetwork (long id, int layer, bool seperateLayers) {
      List<Node> egoNetwork = new List<Node>();

      Node node = GetNode(id, layer);
      foreach(Edge edge in node.Edges) {
          if (seperateLayers && edge.DestinationLayer != edge.StartLayer) continue;

          Node networkNode = GetNode(edge.DestinationId, edge.DestinationLayer);
          egoNetwork.Add(networkNode);
      }

      return egoNetwork;
    }
    
    private static void PrintEgoNetwork(List<Node> egoNetwork, List<List<String>> output) {
        foreach(Node node in egoNetwork) {
            output.Add(new List<String>{node.Layer + "/" + node.Id});
        }
    }

    private long GetCellId (long id, int layer) {
        string nodeName = "n" + id + "l" + layer;
        return HashHelper.HashString2Int64(nodeName);
    }

    private Node GetNode(long id, int layer) {
        return Global.CloudStorage.LoadNode(GetCellId(id, layer));
    }
  }
}
