using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using Trinity;
using Trinity.Network;
using Trinity.Core.Lib;

namespace MultiLayerServer.Algorithms {
  class PageRank {

    private static int UpdatesSent { get; set; }
    private static int UpdatesConfirmed;
    private static Dictionary<long, double> PendingUpdates { get; set; }

    private static int VALUE_RESET_BARRIER = 0;
    private static int AFTER_UPDATE_BARRIER = 1;

    public static void SetInitialValues(double initialValue) {
      foreach(Node_Accessor node in Global.LocalStorage.Node_Accessor_Selector()) {
        node.PageRankData.Value = initialValue;
        node.PageRankData.OldValue = initialValue;
      }
    }

    public static List<double> UpdateRound(bool seperateLayers) {
      UpdatesSent = 0;
      UpdatesConfirmed = 0;
      PendingUpdates = new Dictionary<long, double>();

      foreach(Node_Accessor node in Global.LocalStorage.Node_Accessor_Selector()) {
        node.PageRankData.OldValue = node.PageRankData.Value;
        node.PageRankData.Value = 0;
      }

      Global.CloudStorage.BarrierSync(VALUE_RESET_BARRIER);

      foreach(Node_Accessor node in Global.LocalStorage.Node_Accessor_Selector()) {
        foreach(Edge edge in node.Edges) {
          // If we want to seperate the layers skip edges that go from one layer to another.
          if (seperateLayers && edge.StartLayer != edge.DestinationLayer) {
            continue;
          }
          long targetCellId = Util.GetCellId(edge.DestinationId, edge.DestinationLayer);

          if (Global.CloudStorage.IsLocalCell(targetCellId)) {
              using (Node_Accessor targetNode = Global.LocalStorage.UseNode(targetCellId)) {
                targetNode.PageRankData.Value += node.PageRankData.OldValue;
              }
          } else {
            if (PendingUpdates.ContainsKey(targetCellId)) {
              PendingUpdates[targetCellId] += node.PageRankData.OldValue;
            } else {
              PendingUpdates[targetCellId] = node.PageRankData.OldValue;
            }
          }
        }
      }

      // For each update that needs to be done on a remote server send an update message containig the update info.
      // We also need to send our partition id so the remote server can send the ack back to us.
      foreach(KeyValuePair<long, double> pendingUpdate in PendingUpdates) {
        using (var msg = new PageRankRemoteUpdateMessageWriter(pendingUpdate.Value, pendingUpdate.Key, Global.MyPartitionId)) {
          UpdatesSent++;
          int targetServer = Global.CloudStorage.GetPartitionIdByCellId(pendingUpdate.Key);
          MultiLayerServer.MessagePassingExtension.PageRankRemoteUpdate(Global.CloudStorage[targetServer], msg);
        }
      }

      // Wait until all remote updates are done.
      SpinWait wait = new SpinWait();
      while(UpdatesSent != UpdatesConfirmed) {
        wait.SpinOnce();
      }
      
      
      Global.CloudStorage.BarrierSync(AFTER_UPDATE_BARRIER);

      double valueSum = 0;
      foreach(Node node in Global.LocalStorage.Node_Selector()) {
        valueSum += node.PageRankData.Value;
      }

      List<double> result = new List<double>();
      result.Add(valueSum);

      return result;
    }


    public static void RemoteUpdate (double value, long target) {
  	  // Updates the specified node
      using (Node_Accessor node = Global.LocalStorage.UseNode(target)) {
        node.PageRankData.Value += value;
      }
    }

    public static void RemoteUpdateAnswer() {
      // There can arrive many ack at the same time. So we need to make sure to lock
      // the updatesConfimed to avoid lost updates.
      Interlocked.Increment(ref UpdatesConfirmed);
    }
    


    public static List<double> Normalization (double sum) {
      double normFactor = 1 / Math.Sqrt(sum);
      double delta = 0;

      foreach(Node_Accessor node in Global.LocalStorage.Node_Accessor_Selector()) {
        node.PageRankData.Value *= normFactor;
        delta += Math.Abs(node.PageRankData.Value - node.PageRankData.OldValue);
      }

      List<double> result = new List<double>();
      result.Add(delta);

      return result;  
    }


    public static List<long> TopNodes (int numberOfTopNodes) {
      List<long> topNodes = new List<long>();
      topNodes = Global.LocalStorage.Node_Selector().OrderByDescending(node => node.PageRankData.Value).Take(numberOfTopNodes).Select(node => node.CellId).ToList();

      return topNodes;
    }

  }
}
