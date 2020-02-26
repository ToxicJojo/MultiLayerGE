
using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using Trinity;
using Trinity.Network;
using Trinity.Core.Lib;

namespace MultiLayerServer.Algorithms {
  class HITS {

    private static int HubUpdatesSent { get; set; }

    private static int HubUpdatesConfirmed;

    private static Dictionary<long, double> PendingHubUpdates { get; set; }

    private static int HUB_VALUE_RESET_BARRIER = 0;

    public static void SetInitialValues(double initialValue) {
      foreach(Node_Accessor node in Global.LocalStorage.Node_Accessor_Selector()) {
        node.HITSData.HubScore = initialValue;
        node.HITSData.OldHubScore = initialValue;
        node.HITSData.AuthorityScore = initialValue;
        node.HITSData.OldAuthorityScore = initialValue;
        }
    }


    public static List<double> HubUpdateRound(bool seperateLayers) {
      HubUpdatesSent = 0;
      HubUpdatesConfirmed = 0;
      PendingHubUpdates = new Dictionary<long, double>();

      foreach(Node_Accessor node in Global.LocalStorage.Node_Accessor_Selector()) {
        node.HITSData.OldHubScore = node.HITSData.HubScore;
        node.HITSData.HubScore = 0;
      }


      Global.CloudStorage.BarrierSync(HUB_VALUE_RESET_BARRIER);

      foreach(Node node in Global.LocalStorage.Node_Selector()) {
        foreach(Edge edge in node.Edges) {
          if (seperateLayers && edge.StartLayer != edge.DestinationLayer) {
            continue;
          }

          long targetCellId = Util.GetCellId(edge.DestinationId, edge.DestinationLayer);

          if (Global.CloudStorage.IsLocalCell(targetCellId)) {
            using (Node_Accessor targetNode = Global.LocalStorage.UseNode(targetCellId)) {
              targetNode.HITSData.HubScore += node.HITSData.AuthorityScore;
            }
          } else {
            if (PendingHubUpdates.ContainsKey(targetCellId)) {
              PendingHubUpdates[targetCellId] += node.HITSData.AuthorityScore;
            } else {
              PendingHubUpdates[targetCellId] = node.HITSData.AuthorityScore;
            }
          }
        }
      }

      // For each update that needs to be done on a remote server send an update message containig the update info.
      // We also need to send our partition id so the remote server can send the ack back to us.
      foreach(KeyValuePair<long, double> pendingUpdate in PendingHubUpdates) {
        using (var msg = new HITSRemoteUpdateMessageWriter(pendingUpdate.Value, pendingUpdate.Key, Global.MyPartitionId)) {
          HubUpdatesSent++;
          int targetServer = Global.CloudStorage.GetPartitionIdByCellId(pendingUpdate.Key);
          MultiLayerServer.MessagePassingExtension.HITSHubRemoteUpdate(Global.CloudStorage[targetServer], msg);
        }
      }

      // Wait until all remote updates are done.
      SpinWait wait = new SpinWait();
      while(HubUpdatesSent != HubUpdatesConfirmed) {
        wait.SpinOnce();
      }


      Global.CloudStorage.BarrierSync(1);

      double hubSum = 0;
      foreach(Node node in Global.LocalStorage.Node_Selector()) {
        hubSum += node.HITSData.HubScore;
      }

      List<double> result = new List<double>();
      result.Add(hubSum);

      return result;      
    }


    public static void RemoteHubUpdate (double value, long target) {
      using (Node_Accessor node = Global.LocalStorage.UseNode(target)) {
        node.HITSData.HubScore += value;
      }
    }

    public static void RemoteHubUpdateAnswer () {
      Interlocked.Increment(ref HubUpdatesConfirmed);
    }


    public static List<double> HubNormalization(double hubSum) {
      double normFactor = 1 / Math.Sqrt(hubSum);
      double delta = 0;

      foreach(Node_Accessor node in Global.LocalStorage.Node_Accessor_Selector()) {
        node.HITSData.HubScore *= normFactor;
        delta += Math.Abs(node.HITSData.HubScore - node.HITSData.OldHubScore);
      }

      List<double> result = new List<double>();
      result.Add(delta);

      return result;
    }

  }
}
