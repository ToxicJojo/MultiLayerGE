using System;
using System.Collections.Generic;
using System.Threading;
using Trinity;
using Trinity.TSL.Lib;
using MultiLayerLib;
using MultiLayerLib.MultiLayerServer;

namespace MultiLayerServer.Algorithms {
  class Degree {

    private static int UpdatesSent { get; set; }

    private static int UpdatesConfirmed;

    private static Dictionary<int, Dictionary<long, double>> RemoteUpdates { get; set; }

    public static void GetOutDegree(bool seperateLayers) {
      foreach(Node_Accessor node in Global.LocalStorage.Node_Accessor_Selector()) {
        if (seperateLayers) {
          foreach(Edge edge in node.Edges) {
            if (edge.StartLayer == edge.DestinationLayer) {
              node.DegreeData.OutDegree++;
            } 
          }
        } else {
          node.DegreeData.OutDegree = node.Edges.Count;
        }
      }
    }

    public static void GetInDegree (bool seperateLayers) {
      UpdatesSent = 0;
      UpdatesConfirmed = 0;
      RemoteUpdates = new Dictionary<int, Dictionary<long, double>>();
      for (int i = 0; i < Global.ServerCount; i++) {
        if (i != Global.MyPartitionId) {
          RemoteUpdates[i] = new Dictionary<long, double>();
        }
      }


      foreach(Node_Accessor node in Global.LocalStorage.Node_Accessor_Selector()) {
        foreach(Edge edge in node.Edges) {

          // Don't allow self references
          if (edge.StartId == edge.DestinationId && edge.StartLayer == edge.DestinationLayer) {
            continue;
          }

          // If we want to seperate the layers skip edges that go from one layer to another.
          if (seperateLayers && edge.StartLayer != edge.DestinationLayer) {
            continue;
          }

          long targetCell = Graph.GetCellId(edge.DestinationId, edge.DestinationLayer);

          if (Global.CloudStorage.IsLocalCell(targetCell)) {
            using (Node_Accessor targetNode = Global.LocalStorage.UseNode(targetCell, CellAccessOptions.ReturnNullOnCellNotFound)) {
              if (targetNode != null) {
                targetNode.DegreeData.InDegree++;
              }
            }
          } else {
            int remoteServerId = Global.CloudStorage.GetPartitionIdByCellId(targetCell);
            if (RemoteUpdates[remoteServerId].ContainsKey(targetCell)) {
              RemoteUpdates[remoteServerId][targetCell]++;
            } else {
              RemoteUpdates[remoteServerId][targetCell] = 1;
            }
          }
        }
      }

      // For each server build a list of pairs that contain the information to update the remote nodes.
      // Then sent an update requests to that server.
      foreach(KeyValuePair<int, Dictionary<long, double>> updateCollections in RemoteUpdates) {
        UpdatesSent++;
        List<MultiLayerLib.KeyValuePair> updatePairs = new List<MultiLayerLib.KeyValuePair>();
        foreach(KeyValuePair<long, double> pendingUpdate in updateCollections.Value) {
          updatePairs.Add(new MultiLayerLib.KeyValuePair(pendingUpdate.Key, pendingUpdate.Value));
        }

        using (var msg = new RemoteBulkUpdateMessageWriter(Global.MyPartitionId, updatePairs)) {
          MessagePassingExtension.DegreeBulkUpdate(Global.CloudStorage[updateCollections.Key], msg);
        }
      }

      // Wait until all remote updates are done.
      SpinWait wait = new SpinWait();
      while(UpdatesSent != UpdatesConfirmed) {
        wait.SpinOnce();
      }
    }

    public static void RemoteBulkUpdate (List<MultiLayerLib.KeyValuePair> updates) {
      foreach(MultiLayerLib.KeyValuePair update in updates) {
        using (Node_Accessor node = Global.LocalStorage.UseNode(update.Key, CellAccessOptions.ReturnNullOnCellNotFound)) {
          if (node != null) {
            node.DegreeData.InDegree += Convert.ToInt32(update.Value);
          }
        }
      }
    }

    public static void RemoteUpdateAnswer () {
      Interlocked.Increment(ref UpdatesConfirmed);
    }

    public static void GetTotalDegree () {
      foreach(Node_Accessor node in Global.LocalStorage.Node_Accessor_Selector()) {
        node.DegreeData.TotalDegree = node.DegreeData.InDegree + node.DegreeData.OutDegree;
      }
    }
  }
}
