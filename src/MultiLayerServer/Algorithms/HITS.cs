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


    private static int AuthUpdatesSent { get; set; }
    private static int AuthUpdatesConfirmed;
    private static Dictionary<long, double> PendingAuthUpdates { get; set; }

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

      Dictionary<long, double> remoteAuthScores = new Dictionary<long, double>();
      HashSet<long> remoteKeys = new HashSet<long>();

      foreach(Node_Accessor node in Global.LocalStorage.Node_Accessor_Selector()) {
        foreach(Edge edge in node.Edges) {
          if (seperateLayers && edge.StartLayer != edge.DestinationLayer) {
            continue;
          }

          long targetCellId = Util.GetCellId(edge.DestinationId, edge.DestinationLayer);

          

          if (Global.CloudStorage.IsLocalCell(targetCellId)) {
            Node targetNode = Global.LocalStorage.LoadNode(targetCellId);
            node.HITSData.HubScore += targetNode.HITSData.AuthorityScore;
          } else {
            remoteKeys.Add(targetCellId);
          }
        }
      }

      Global.CloudStorage.BarrierSync(4);

      foreach(long remoteKey in remoteKeys) {
        remoteAuthScores[remoteKey] = Global.CloudStorage.LoadNode(remoteKey).HITSData.AuthorityScore;
      }

      Global.CloudStorage.BarrierSync(5);

      foreach(Node_Accessor node in Global.LocalStorage.Node_Accessor_Selector()) {
        foreach(Edge edge in node.Edges) {
          if (seperateLayers && edge.StartLayer != edge.DestinationLayer) {
            continue;
          }

          long targetCellId = Util.GetCellId(edge.DestinationId, edge.DestinationLayer);
          if (!Global.CloudStorage.IsLocalCell(targetCellId)) {
            node.HITSData.HubScore += remoteAuthScores[targetCellId];
          }
        }
      }

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



    public static List<double> AuthUpdateRound(bool seperateLayers) {
      AuthUpdatesSent = 0;
      AuthUpdatesConfirmed = 0;
      PendingAuthUpdates = new Dictionary<long, double>();

      foreach(Node_Accessor node in Global.LocalStorage.Node_Accessor_Selector()) {
        node.HITSData.OldAuthorityScore = node.HITSData.AuthorityScore;
        node.HITSData.AuthorityScore = 0;
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
              targetNode.HITSData.AuthorityScore += node.HITSData.HubScore;
            }
          } else {
            if (PendingAuthUpdates.ContainsKey(targetCellId)) {
              PendingAuthUpdates[targetCellId] += node.HITSData.HubScore;
            } else {
              PendingAuthUpdates[targetCellId] = node.HITSData.HubScore;
            }
          }
        }
      }

      // For each update that needs to be done on a remote server send an update message containig the update info.
      // We also need to send our partition id so the remote server can send the ack back to us.
      foreach(KeyValuePair<long, double> pendingUpdate in PendingAuthUpdates) {
        using (var msg = new HITSRemoteUpdateMessageWriter(pendingUpdate.Value, pendingUpdate.Key, Global.MyPartitionId)) {
          AuthUpdatesSent++;
          int targetServer = Global.CloudStorage.GetPartitionIdByCellId(pendingUpdate.Key);
          MultiLayerServer.MessagePassingExtension.HITSAuthRemoteUpdate(Global.CloudStorage[targetServer], msg);
        }
      }

      // Wait until all remote updates are done.
      SpinWait wait = new SpinWait();
      while(AuthUpdatesSent != AuthUpdatesConfirmed) {
        wait.SpinOnce();
      }


      Global.CloudStorage.BarrierSync(1);

      double AuthSum = 0;
      foreach(Node node in Global.LocalStorage.Node_Selector()) {
        AuthSum += node.HITSData.AuthorityScore;
      }

      List<double> result = new List<double>();
      result.Add(AuthSum);

      return result;      
    }



    public static void RemoteAuthUpdate (double value, long target) {
      using (Node_Accessor node = Global.LocalStorage.UseNode(target)) {
        node.HITSData.AuthorityScore += value;
      }
    }

    public static void RemoteAuthUpdateAnswer () {
      Interlocked.Increment(ref AuthUpdatesConfirmed);
    }


    public static List<double> AuthNormalization(double authSum) {
      double normFactor = 1 / Math.Sqrt(authSum);
      double delta = 0;

      foreach(Node_Accessor node in Global.LocalStorage.Node_Accessor_Selector()) {
        node.HITSData.AuthorityScore *= normFactor;
        delta += Math.Abs(node.HITSData.AuthorityScore - node.HITSData.OldAuthorityScore);
      }

      List<double> result = new List<double>();
      result.Add(delta);

      return result;
    }
  }
}
