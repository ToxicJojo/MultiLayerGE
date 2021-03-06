using System.Collections.Generic;
using Trinity;
using Trinity.TSL.Lib;
using System;
using MultiLayerProxy.Proxy;
using Trinity.Core.Lib;
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

    private List<Node> incomingNetwork;

    private List<Node> outgoingNetwork;


    public EgoNetwork (MultiLayerProxyImpl proxy, long id, int layer, bool seperateLayers): base(proxy) {
      SeperateLayers = seperateLayers;
      Id = id;
      Layer = layer;
      this.Name = "EgoNetwork";
    }

    public override void Run() {
      
      outgoingNetwork = GetEgoNetwork(Id, Layer, SeperateLayers);

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

      incomingNetwork = new List<Node>();
      foreach(long id in incomingIds) {
        incomingNetwork.Add(Global.CloudStorage.LoadNode(id));
      }

   }

    public override List<List<string>>  GetResultTable(OutputOptions options) {
      List<List<String>> output = new List<List<string>>();

      output.Add(new List<string> {"Outgoing: "});
      PrintEgoNetwork(outgoingNetwork, output);
      output.Add(new List<string> {"Incoming: "});
      PrintEgoNetwork(incomingNetwork, output);

      return output;
    }
    

    private List<Node> GetEgoNetwork (long id, int layer, bool seperateLayers) {
      List<Node> egoNetwork = new List<Node>();

      Node node = Graph.LoadNode(id, layer);
      foreach(Edge edge in node.Edges) {
          if (seperateLayers && edge.DestinationLayer != edge.StartLayer) continue;

          Node networkNode = Graph.LoadNode(edge.DestinationId, edge.DestinationLayer);
          egoNetwork.Add(networkNode);
      }

      return egoNetwork;
    }
    
    private static void PrintEgoNetwork(List<Node> egoNetwork, List<List<String>> output) {
        foreach(Node node in egoNetwork) {
            output.Add(new List<String>{node.Layer + "/" + node.Id});
        }
    }
  }
}
