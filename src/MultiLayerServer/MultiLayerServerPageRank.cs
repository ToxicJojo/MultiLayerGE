using System;
using System.Collections.Generic;
using System.Threading;
using Trinity;
using Trinity.Network;

namespace MultiLayerServer {

  public partial class MultiLayerServerImpl: MultiGraphServerBase {
    // Barrier Constants 
    private int PAGE_RANK_VALUE_RESET = 0;
    private int PAGE_RANK_AFTER_UPDATE = 1;


    /// <summary>
    /// Sets the initial page rank value for every node the server holds.
    /// </summary>
    /// <param name="request">The request containing the initial value.</param>
    public override void SetInitialValueHandler(SetInitialValueMessageReader request) {
      foreach(Node_Accessor node in Global.LocalStorage.Node_Accessor_Selector()) {
        node.PageRankData.Value = request.initialValue;
        node.PageRankData.OldValue = request.initialValue;
      }

      PhaseFinished("phasePageRankInitialValues");
    }


    // During the update phase we buffer updates for nodes that are on other servers.
    // These updates are saved in pendingUpdates, where the long represents the node id
    // and the double is the amount the nodes value has to be increased by.
    Dictionary<long, double> pendingUpdates = new Dictionary<long, double>();
    // As updates are processed asynchrounosly we need to keep count on how many we sent and for how many
    // we received a confirmation that they are completed.
    int updatesSend;
    int updatesConfirmed;
    /// <summary>
    /// Does one round of page rank updates. 
    /// Once the update is done the sum of all squared pake rank values is send to the proxy
    /// which is needed to normalize the values.
    /// </summary>
    public override void PageRankUpdateHandler() {
      // Resey the updatesSend/Confirmed back to 0.
      updatesSend = 0;
      updatesConfirmed = 0;

      // Set the page rank values to zero and update the old value.
      foreach(Node_Accessor node in Global.LocalStorage.Node_Accessor_Selector()) {
        node.PageRankData.OldValue = node.PageRankData.Value;
        node.PageRankData.Value = 0;
      }

      // Wait until all servers are done updating the old value and setting the current value to 0.
      Global.CloudStorage.BarrierSync(PAGE_RANK_VALUE_RESET);

      // Update every value
      foreach(Node_Accessor node in Global.LocalStorage.Node_Accessor_Selector()) {
        foreach(Edge edge in node.Edges) {
          // TODO I need to implement a way to tell the algorithm to only use edges
          // that don't jump layers.
          long referenceCellId = Util.GetCellId(edge.DestinationId, edge.DestinationLayer);

          // If the node we need to update is a local node we can to the update.
          // Otherwise we need to send a message to the server that holds that node.
          if (Global.CloudStorage.IsLocalCell(referenceCellId)) {
            using(var referencedNode = Global.LocalStorage.UseNode(referenceCellId)) {
              referencedNode.PageRankData.Value += node.PageRankData.OldValue;
            }
          } else {
            // Buffer the update for the remote node in pendingUpdates. 
            if (pendingUpdates.ContainsKey(referenceCellId)) {
              pendingUpdates[referenceCellId] += node.PageRankData.OldValue;
            } else {
              pendingUpdates[referenceCellId] = node.PageRankData.OldValue;
            }
          }
        }
      }

      // For each update that needs to be done on a remote server send an update message containig the update info.
      // We also need to send our partition id so the remote server can send the ack back to us.
      foreach(KeyValuePair<long, double> pendingUpdate in pendingUpdates) {
        using (var msg = new PageRankRemoteUpdateMessageWriter(pendingUpdate.Value, pendingUpdate.Key, Global.MyPartitionId)) {
          this.updatesSend++;
          int targetServer = Global.CloudStorage.GetPartitionIdByCellId(pendingUpdate.Key);
          MultiGraphServer.MessagePassingExtension.PageRankRemoteUpdate(Global.CloudStorage[targetServer], msg);
        }
      }

      // Wait until all remote updates are done.
      SpinWait wait = new SpinWait();
      while(updatesSend != updatesConfirmed) {
        wait.SpinOnce();
      }

      Global.CloudStorage.BarrierSync(PAGE_RANK_AFTER_UPDATE);


      // To be able to normalize the page rank values later we need to calculate the sum of all values squared.
      double valueSum = 0;
      foreach(Node node in Global.LocalStorage.Node_Selector()) {
        valueSum += node.PageRankData.Value * node.PageRankData.Value;
      }

      List<double> result = new List<double>();
      result.Add(valueSum);

      // Send the sum of squared values back to the proxy.
      PhaseFinished("phasePageRankUpdate", result);
    }



    /// <summary>
    /// Handles an update request of a remote server.
    /// </summary>
    /// <param name="request">The request that contains the info about which node needs to be update by which value and who sent the request.</param>
    public override void PageRankRemoteUpdateHandler(PageRankRemoteUpdateMessageReader request) {
      // Updates the specified node
      using (Node_Accessor node = Global.LocalStorage.UseNode(request.Target)) {
        node.PageRankData.Value += request.Value;
      }

      // Send an ack to the remote server.
      Global.CloudStorage.PageRankRemoteUpdateAnswerToMultiGraphServer(request.From);
    }

    /// <summary>
    /// Handles acks for remote updates.
    /// </summary>
    public override void PageRankRemoteUpdateAnswerHandler() {
      // There can arrive many ack at the same time. So we need to make sure to lock
      // the updatesConfimed to avoid lost updates.
      Interlocked.Increment(ref updatesConfirmed);
    }

    /// <summary>
    /// Normalizes the page rank value of each local node according to the total sum of all page rank values.
    /// Once the normalization is done the total delta between the new and old values is send to the 
    /// </summary>
    /// <param name="request">The request that contains the total sum of all page rank values.</param>
    public override void PageRankNormalizationHandler(PageRankNormalizationMessageReader request) {
      double normFactor = 1 / Math.Sqrt(request.Sum);
      double delta = 0;

      foreach(Node_Accessor node in Global.LocalStorage.Node_Accessor_Selector()) {
        node.PageRankData.Value *= normFactor;
        delta += Math.Abs(node.PageRankData.Value - node.PageRankData.OldValue);
      }

      List<double> result = new List<double>();
      result.Add(delta);
      
      PhaseFinished("phasePageRankNormalization", result);
    }
  }
}
