using Trinity;
using System.Collections.Generic;
using MultiLayerLib;

namespace MultiLayerServer.Algorithms {

  class EgoNetwork {

    public static List<long> GetIncomingNetwork(long id, int layer, bool seperateLayers) {
      List<long> incomingNetwork = new List<long>();
      foreach(Node_Accessor node in Global.LocalStorage.Node_Accessor_Selector()) {
        foreach(Edge edge in node.Edges) {
          if (seperateLayers && edge.DestinationLayer != edge.StartLayer) continue;

          if (edge.DestinationId == id && edge.DestinationLayer == layer) {
            incomingNetwork.Add(node.CellId);
          }
        }
      }

      return incomingNetwork;
    }   
  }
}
