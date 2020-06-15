using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using Trinity;
using Trinity.Network;
using Trinity.TSL.Lib;
using MultiLayerLib;
using MultiLayerLib.MultiLayerServer;

namespace MultiLayerServer.Algorithms {
  class PageRank {

    // We need to keep track of the number of updates for remote nodes we sent out
    // and how many of them have finished.
    private static int UpdatesSent { get; set; }
    private static int UpdatesConfirmed;
    // We buffer updates for remote nodes in this dictionary.
    // The keys are the serverId while the value is a dictionary that contains the update information for the nodes that belong to the keyed server.
    // The update information is a dictinary that consists of the node id as a the key and the amount its pagerank value needs to be increased by.
    private static Dictionary<int ,Dictionary<long, double>> RemoteUpdates { get; set; }

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
      RemoteUpdates = new Dictionary<int, Dictionary<long, double>>();
      for (int i = 0; i < Global.ServerCount; i++) {
        if (i != Global.MyPartitionId) {
          RemoteUpdates[i] = new Dictionary<long, double>();
        }
      }

      // We need to remember the old value and set the current one to 0.
      foreach(Node_Accessor node in Global.LocalStorage.Node_Accessor_Selector()) {
        node.PageRankData.OldValue = node.PageRankData.Value;
        node.PageRankData.Value = 0;
      }

      // Wait until all servers are done with resetting the current value.
      Global.CloudStorage.BarrierSync(VALUE_RESET_BARRIER);

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
          long targetCellId = Graph.GetCellId(edge.DestinationId, edge.DestinationLayer);

          // If the target node is a local one we can do the update directly.
          if (Global.CloudStorage.IsLocalCell(targetCellId)) {
              using (Node_Accessor targetNode = Global.LocalStorage.UseNode(targetCellId, CellAccessOptions.ReturnNullOnCellNotFound)) {
                if (targetNode != null) {
                  targetNode.PageRankData.Value += node.PageRankData.OldValue;
                }
              }
          } else {
            // Save the remote update for the targetCell.
            int remoteServerId = Graph.GetNodePartition(targetCellId);
            if (RemoteUpdates[remoteServerId].ContainsKey(targetCellId)) {
              RemoteUpdates[remoteServerId][targetCellId] += node.PageRankData.OldValue;
            } else {
              RemoteUpdates[remoteServerId][targetCellId] = node.PageRankData.OldValue;
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
          MessagePassingExtension.PageRankRemoteBulkUpdate(Global.CloudStorage[updateCollections.Key], msg);
        }
      }


      // Wait until all remote updates are done.
      SpinWait wait = new SpinWait();
      while(UpdatesSent != UpdatesConfirmed) {
        wait.SpinOnce();
      }
      
      
      Global.CloudStorage.BarrierSync(AFTER_UPDATE_BARRIER);

      double valueSum = 0;
      foreach(Node node in Global.LocalStorage.Node_Accessor_Selector()) {
        valueSum += node.PageRankData.Value;
      }

      List<double> result = new List<double>();
      result.Add(valueSum);
      Console.WriteLine("ValueSum: {0}", valueSum);

      return result;
    }

    public static void RemoteBulkUpdate (List<MultiLayerLib.KeyValuePair> updates) {
      foreach(MultiLayerLib.KeyValuePair update in updates) {
        using (Node_Accessor node = Global.LocalStorage.UseNode(update.Key, CellAccessOptions.ReturnNullOnCellNotFound)) {
          if (node != null) {
            node.PageRankData.Value += update.Value;
          }
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
