using Trinity;
using MultiLayerServer.Algorithms;
using System.Collections.Generic;
using MultiLayerLib;
using MultiLayerLib.MultiLayerServer;

namespace MultiLayerServer.Server {
  public partial class MultiLayerServerImpl: MultiLayerServerBase {

    public override void PageRankSetInitialValuesHandler(PageRankSetInitialValuesMessageReader request) {
      PageRank.SetInitialValues(request.InitialValue);

      PhaseFinished(Phases.PageRankInitialValues);
    }

    public override void PageRankUpdateRoundHandler(PageRankUpdateMessageReader request) {
      List<double> updateResult = PageRank.UpdateRound(request.SeperateLayers);

      PhaseFinished(Phases.PageRankUpdateRound, Util.ToStringList(updateResult));
    }

    public override void PageRankNormalizationHandler(PageRankNormalizationMessageReader request) {
      List<double> normalizationResult = PageRank.Normalization(request.Sum);

      PhaseFinished(Phases.PageRankNormalization, Util.ToStringList(normalizationResult));
    }

    public override void PageRankRemoteUpdateAnswerHandler() {
      PageRank.RemoteUpdateAnswer();
    }

    public override void PageRankTopNodesServerHandler(PageRankTopNodesServerMessageReader request) {
      List<long> topNodes = PageRank.TopNodes(request.NumberOfTopNodes, request.SeperateLayers);

      PhaseFinished(Phases.PageRankTopNodes, Util.ToStringList(topNodes));
    }

    public override void PageRankRemoteBulkUpdateHandler(RemoteBulkUpdateMessageReader request) {
      PageRank.RemoteBulkUpdate(request.Values);
      // After we have done the update we need to tell the other server that we finished it.
      var server = Global.CloudStorage[request.From];
      MessagePassingExtension.PageRankRemoteUpdateAnswer(server);
    }

  }
}
