using System.Collections.Generic;
using MultiLayerLib;

namespace MultiLayerServer.Algorithms {
  class Stats {

    /// <summary>
    /// Calculates the local node count for every layer of the graph.
    /// </summary>
    /// <returns>A list that contains the local node count for every layer of the graph.</returns>
    public static List<long> GetNodeCount() {
      long[] nodeCount = new long[Graph.LayerCount];

      foreach(Node_Accessor node in Graph.NodeAccessor()) {
        nodeCount[node.Layer - 1]++;
      }

      List<long> result = new List<long>(nodeCount);
      return result;
    }

    /// <summary>
    /// Calculates the local edge count for every layer of the graph.
    /// </summary>
    /// <returns>A list that contains the local edge count for every layer of the graph.</returns>
    public static List<long> GetEdgeCount() {
      long[] edgeCount = new long[Graph.LayerCount];

      foreach(Node node in Graph.NodeAccessor()) {
        foreach(Edge edge in node.Edges) {
          // Only count edges which are on the same layer.
          if (edge.StartLayer == edge.DestinationLayer) {
            edgeCount[edge.StartLayer - 1]++;
          }
        }
      }

      List<long> result = new List<long>(edgeCount);
      return result;
    }
  }
}
