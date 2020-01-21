using System;
using System.Collections.Generic;
using Trinity;
using Trinity.Network;

namespace MultiLayerServer {

  public partial class MultiLayerServerImpl: MultiGraphServerBase {
  private int PAGE_RANK_VALUE_RESET = 0;
  private int PAGE_RANK_AFTER_UPDATE = 0;

    public override void SetInitialValueHandler(SetInitialValueMessageReader request) {
      foreach(Node_Accessor node in Global.LocalStorage.Node_Accessor_Selector()) {
        node.PageRankData.Value = request.initialValue;
        node.PageRankData.OldValue = request.initialValue;
      }

      PhaseFinished("phasePageRankInitialValues");
    }

    public override void PageRankUpdateHandler() {
      // Set the page rank values to zero
      foreach(Node_Accessor node in Global.LocalStorage.Node_Accessor_Selector()) {
        node.PageRankData.OldValue = node.PageRankData.Value;
        node.PageRankData.Value = 0;
      }

      Global.CloudStorage.BarrierSync(PAGE_RANK_VALUE_RESET);

      Console.WriteLine("After Barrier");

      // Update every value
      foreach(Node_Accessor node in Global.LocalStorage.Node_Accessor_Selector()) {
        foreach(Edge edge in node.Edges) {
          // TODO I need to implement a way to tell the algorithm to only use edges
          // that don't jump layers.
          long referenceCellId = Util.GetCellId(edge.DestinationId, edge.DestinationLayer);

          if (Global.CloudStorage.IsLocalCell(referenceCellId)) {
            using(var referencedNode = Global.LocalStorage.UseNode(referenceCellId)) {
              referencedNode.PageRankData.Value += node.PageRankData.OldValue;
            }
          } else {
            // TODO Bundle updates for other servers and send them.
            
          }
        }
      }


      Global.CloudStorage.BarrierSync(PAGE_RANK_AFTER_UPDATE);

      double valueSum = 0;
      foreach(Node node in Global.LocalStorage.Node_Selector()) {
        valueSum += node.PageRankData.Value * node.PageRankData.Value;
      }

      List<double> result = new List<double>();
      result.Add(valueSum);

      PhaseFinished("phasePageRankUpdate", result);
    }


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
