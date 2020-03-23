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

    private static Dictionary<int ,Dictionary<long, double>> RemoteUpdates { get; set; }

    private static int VALUE_RESET_BARRIER = 0;
    private static int AFTER_UPDATE_BARRIER = 1;

    public static void SetInitialValues(double initialValue) {
      foreach(Node_Accessor node in Global.LocalStorage.Node_Accessor_Selector()) {
        node.PageRankData.Value = initialValue;
        node.PageRankData.OldValue = initialValue;
      }
      Console.WriteLine("Inital Values DOne");
    }

    public static List<double> UpdateRound(bool seperateLayers) {
      UpdatesSent = 0;
      UpdatesConfirmed = 0;
      PendingUpdates = new Dictionary<long, double>();
      RemoteUpdates = new Dictionary<int, Dictionary<long, double>>();
      for (int i = 0; i < Global.ServerCount; i++) {
        if (i != Global.MyPartitionId) {
          RemoteUpdates[i] = new Dictionary<long, double>();
        }
      }

      foreach(Node_Accessor node in Global.LocalStorage.Node_Accessor_Selector()) {
        node.PageRankData.OldValue = node.PageRankData.Value;
        node.PageRankData.Value = 0;
      }

      Global.CloudStorage.BarrierSync(VALUE_RESET_BARRIER);
      Console.WriteLine("Value reset  DOne");

      long nodesDoneCount = 0;

      foreach(Node_Accessor node in Global.LocalStorage.Node_Accessor_Selector()) {
        foreach(Edge edge in node.Edges) {
          // If we want to seperate the layers skip edges that go from one layer to another.
          if (seperateLayers && edge.StartLayer != edge.DestinationLayer) {
            continue;
          }

          // Don't allow self references
          if (edge.StartId == edge.DestinationId && edge.StartLayer == edge.DestinationLayer) {
            continue;
          }
          long targetCellId = Util.GetCellId(edge.DestinationId, edge.DestinationLayer);

          if (Global.CloudStorage.IsLocalCell(targetCellId)) {
              try {
                using (Node_Accessor targetNode = Global.LocalStorage.UseNode(targetCellId)) {
                 targetNode.PageRankData.Value += node.PageRankData.OldValue;
                }
              } catch (Exception e) {
              }
          } else {
            // Disable remote updates for testing
            /*
            if (PendingUpdates.ContainsKey(targetCellId)) {
              PendingUpdates[targetCellId] += node.PageRankData.OldValue;
            } else {
              PendingUpdates[targetCellId] = node.PageRankData.OldValue;
            }
            */
            int remoteServerId = Global.CloudStorage.GetPartitionIdByCellId(targetCellId);
            if (RemoteUpdates[remoteServerId].ContainsKey(targetCellId)) {
              RemoteUpdates[remoteServerId][targetCellId] += node.PageRankData.OldValue;
            } else {
              RemoteUpdates[remoteServerId][targetCellId] = node.PageRankData.OldValue;
            }
          }
        }
        nodesDoneCount++;
        if (nodesDoneCount % 10000 == 0) {
        }
      }


      /*
      // For each update that needs to be done on a remote server send an update message containig the update info.
      // We also need to send our partition id so the remote server can send the ack back to us.
      foreach(KeyValuePair<long, double> pendingUpdate in PendingUpdates) {
        using (var msg = new PageRankRemoteUpdateMessageWriter(pendingUpdate.Value, pendingUpdate.Key, Global.MyPartitionId)) {
          UpdatesSent++;
          int targetServer = Global.CloudStorage.GetPartitionIdByCellId(pendingUpdate.Key);
          MultiLayerServer.MessagePassingExtension.PageRankRemoteUpdate(Global.CloudStorage[targetServer], msg);
        }
      }
      */

      foreach(KeyValuePair<int, Dictionary<long, double>> updateCollections in RemoteUpdates) {
        UpdatesSent++;
        List<PageRankUpdatePair> updatePairs = new List<PageRankUpdatePair>();
        foreach(KeyValuePair<long, double> pendingUpdate in updateCollections.Value) {
          updatePairs.Add(new PageRankUpdatePair(pendingUpdate.Value, pendingUpdate.Key));
        }

        using (var msg = new PageRankRemoteBulkUpdateMessageWriter(Global.MyPartitionId, updatePairs)) {
          MultiLayerServer.MessagePassingExtension.PageRankRemoteBulkUpdate(Global.CloudStorage[updateCollections.Key], msg);
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
      try {
        using (Node_Accessor node = Global.LocalStorage.UseNode(target)) {
          node.PageRankData.Value += value;
        }
      } catch(Exception e) {
        // There are edges that point to nodes that are not loaded in ge. So we need to catch potentioal errors when accessing those nodes.
        // I might be able to circumvent this by using CellAccessOptions
      }
    }

    public static void RemoteBulkUpdate (List<PageRankUpdatePair> updates) {
      foreach(PageRankUpdatePair update in updates) {
        try {
          using (Node_Accessor node = Global.LocalStorage.UseNode(update.NodeId)) {
            node.PageRankData.Value += update.Value;
          }
        } catch (Exception e) {
        }
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


    public static List<long> TopNodes (int numberOfTopNodes, bool seperateLayers) {
      List<long> topNodes = new List<long>();
      if (!seperateLayers) {
        topNodes = Global.LocalStorage.Node_Selector().OrderByDescending(node => node.PageRankData.Value).Take(numberOfTopNodes).Select(node => node.CellId).ToList();
      } else {
        var result = Global.LocalStorage.Node_Selector().GroupBy(node => node.Layer).Select(group => new { Layer = group.Key, Nodes = group.OrderByDescending(node => node.PageRankData.Value).Take(numberOfTopNodes) });
        foreach (var group in result) {
          topNodes.AddRange(group.Nodes.Select(node => node.CellId).ToList());
        }
      }

      return topNodes;
    }

  }
}
