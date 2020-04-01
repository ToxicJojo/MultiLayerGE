using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using Trinity;
using Trinity.Network;
using Trinity.Core.Lib;
using Trinity.TSL.Lib;

namespace MultiLayerServer.Algorithms {
  class HITS {

    private static int AuthUpdatesSent { get; set; }
    private static int AuthUpdatesConfirmed;

    private static int AuthValueRequestsSent { get; set; }
    private static int AuthValueRequestsAnswerd;


    private static Dictionary<long, double> RemoteAuthScores { get; set; }

    private static Dictionary<int, Dictionary<long, double>> RemoteUpdates { get; set; }

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
      AuthValueRequestsSent = 0;
      AuthValueRequestsAnswerd = 0;

      foreach(Node_Accessor node in Global.LocalStorage.Node_Accessor_Selector()) {
        node.HITSData.OldHubScore = node.HITSData.HubScore;
        node.HITSData.HubScore = 0;
      }


      Global.CloudStorage.BarrierSync(HUB_VALUE_RESET_BARRIER);

      RemoteAuthScores = new Dictionary<long, double>();
      HashSet<long> remoteKeys = new HashSet<long>();

      foreach(Node_Accessor node in Global.LocalStorage.Node_Accessor_Selector()) {
        foreach(Edge edge in node.Edges) {
          if (seperateLayers && edge.StartLayer != edge.DestinationLayer) {
            continue;
          }

          if (edge.StartId == edge.DestinationId && edge.StartLayer == edge.DestinationLayer) {
            continue;
          }

          long targetCellId = Util.GetCellId(edge.DestinationId, edge.DestinationLayer);

          if (Global.CloudStorage.IsLocalCell(targetCellId)) {
            // For some reason using the node accessor instead of loading the node is way faster
            // I might want to look into why this is and where I can use this to speed up things
            // It might be because we read directly from ram instead of creating an object that we read from?
            using (Node_Accessor targetNode = Global.LocalStorage.UseNode(targetCellId, CellAccessOptions.ReturnNullOnCellNotFound)) {
              if (targetNode != null) {
                node.HITSData.HubScore += targetNode.HITSData.AuthorityScore;
              }
            }
          } else {
            remoteKeys.Add(targetCellId);
          }
        }
      }

      Global.CloudStorage.BarrierSync(4);

      foreach(var server in Global.CloudStorage) {
        AuthValueRequestsSent++;
        using (var msg = new RemoteBulkGetMessageWriter(Global.MyPartitionId ,remoteKeys.ToList())) {
          MultiLayerServer.MessagePassingExtension.HITSGetBulkAuthValues(server, msg);
        }
      }

      // Wait until all requests are done.
      SpinWait wait = new SpinWait();
      while(AuthUpdatesSent != AuthValueRequestsAnswerd) {
        wait.SpinOnce();
      }

      Global.CloudStorage.BarrierSync(5);
      foreach(Node_Accessor node in Global.LocalStorage.Node_Accessor_Selector()) {
        foreach(Edge edge in node.Edges) {
          if (seperateLayers && edge.StartLayer != edge.DestinationLayer) {
            continue;
          }

          long targetCellId = Util.GetCellId(edge.DestinationId, edge.DestinationLayer);
          if (!Global.CloudStorage.IsLocalCell(targetCellId) && RemoteAuthScores.ContainsKey(targetCellId)) {
            node.HITSData.HubScore += RemoteAuthScores[targetCellId];
          }
        }
      }

      double hubSum = 0;
      foreach(Node node in Global.LocalStorage.Node_Accessor_Selector()) {
        hubSum += node.HITSData.HubScore;
      }

      List<double> result = new List<double>();
      result.Add(hubSum);

      return result;      
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


    public static List<KeyValuePair> GetBulkAuthValues (List<long> ids) {
      List<KeyValuePair> values = new List<KeyValuePair>();
      foreach(Node node in Global.LocalStorage.Node_Accessor_Selector()) {
        values.Add(new KeyValuePair(node.CellId, node.HITSData.AuthorityScore));
      }

      return values;
    }

    public static void AddRemoteAuthScores(List<KeyValuePair> values) {
      foreach(KeyValuePair valuePair in values) {
        RemoteAuthScores[valuePair.Key] = valuePair.Value;
      }

      Interlocked.Increment(ref AuthValueRequestsAnswerd);
    }


    public static List<double> AuthUpdateRound(bool seperateLayers) {
      AuthUpdatesSent = 0;
      AuthUpdatesConfirmed = 0;
      RemoteUpdates = new Dictionary<int, Dictionary<long, double>>();
      for (int i = 0; i < Global.ServerCount; i++) {
        if (i != Global.MyPartitionId) {
          RemoteUpdates[i] = new Dictionary<long, double>();
        }
      }

      foreach(Node_Accessor node in Global.LocalStorage.Node_Accessor_Selector()) {
        node.HITSData.OldAuthorityScore = node.HITSData.AuthorityScore;
        node.HITSData.AuthorityScore = 0;
      }


      Global.CloudStorage.BarrierSync(HUB_VALUE_RESET_BARRIER);

      foreach(Node node in Global.LocalStorage.Node_Accessor_Selector()) {
        foreach(Edge edge in node.Edges) {
          if (seperateLayers && edge.StartLayer != edge.DestinationLayer) {
            continue;
          }

          // Don't allow self references
          if (edge.StartId == edge.DestinationId && edge.StartLayer == edge.DestinationLayer) {
            continue;
          }

          long targetCellId = Util.GetCellId(edge.DestinationId, edge.DestinationLayer);

          if (Global.CloudStorage.IsLocalCell(targetCellId)) {
            using (Node_Accessor targetNode = Global.LocalStorage.UseNode(targetCellId, CellAccessOptions.ReturnNullOnCellNotFound)) {
              if (targetNode != null) {
                targetNode.HITSData.AuthorityScore += node.HITSData.HubScore;
              }
            }
          } else {
            int remoteServerId = Global.CloudStorage.GetPartitionIdByCellId(targetCellId);
            if (RemoteUpdates[remoteServerId].ContainsKey(targetCellId)) {
              RemoteUpdates[remoteServerId][targetCellId] += node.HITSData.HubScore;
            } else {
              RemoteUpdates[remoteServerId][targetCellId] = node.HITSData.HubScore;
            }
          }
        }
      }

      foreach(KeyValuePair<int, Dictionary<long, double>> updateCollections in RemoteUpdates) {
        AuthUpdatesSent++;
        List<KeyValuePair> updatePairs = new List<KeyValuePair>();
        foreach(KeyValuePair<long, double> pendingUpdate in updateCollections.Value) {
          updatePairs.Add(new KeyValuePair(pendingUpdate.Key, pendingUpdate.Value));
        }

        using (var msg = new RemoteBulkUpdateMessageWriter(Global.MyPartitionId, updatePairs)) {
          MultiLayerServer.MessagePassingExtension.HITSRemoteBulkUpdate(Global.CloudStorage[updateCollections.Key], msg);
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

    public static void RemoteBulkAuthUpdate (List<KeyValuePair> updates) {
      foreach(KeyValuePair update in updates) {
        using (Node_Accessor node = Global.LocalStorage.UseNode(update.Key, CellAccessOptions.ReturnNullOnCellNotFound)) {
          if (node != null) {
            node.HITSData.AuthorityScore += update.Value;
          }
        }
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

    public static List<long> TopAuthorities (int numberOfTopNodes, bool seperateLayers) {
      List<long> topNodes = new List<long>();
      if (!seperateLayers) {
        topNodes = Global.LocalStorage.Node_Selector().OrderByDescending(node => node.HITSData.AuthorityScore).Take(numberOfTopNodes).Select(node => node.CellId).ToList();
      } else {
        var result = Global.LocalStorage.Node_Selector().GroupBy(node => node.Layer).Select(group => new { Layer = group.Key, Nodes = group.OrderByDescending(node => node.HITSData.AuthorityScore).Take(numberOfTopNodes) });
        foreach (var group in result) {
          topNodes.AddRange(group.Nodes.Select(node => node.CellId).ToList());
        }
      }

      return topNodes;
    }


    public static List<long> TopHubs (int numberOfTopNodes, bool seperateLayers) {
      List<long> topNodes = new List<long>();
      if (!seperateLayers) {
        topNodes = Global.LocalStorage.Node_Selector().OrderByDescending(node => node.HITSData.HubScore).Take(numberOfTopNodes).Select(node => node.CellId).ToList();
      } else {
        var result = Global.LocalStorage.Node_Selector().GroupBy(node => node.Layer).Select(group => new { Layer = group.Key, Nodes = group.OrderByDescending(node => node.HITSData.HubScore).Take(numberOfTopNodes) });
        foreach (var group in result) {
          topNodes.AddRange(group.Nodes.Select(node => node.CellId).ToList());
        }
      }

      return topNodes;
    }

  }
}
