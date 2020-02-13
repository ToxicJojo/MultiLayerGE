using Trinity;
using MultiLayerServer.Algorithms;
using System.Collections.Generic;

namespace MultiLayerServer.Server {
  public partial class MultiLayerServerImpl: MultiLayerServerBase {

    public override void PageRankSetInitialValuesHandler(PageRankSetInitialValuesMessageReader request) {
      PageRank.SetInitialValues(request.InitialValue);

      PhaseFinished(Phases.PageRankInitialValues);
    }

    public override void PageRankUpdateRoundHandler() {
      List<double> updateResult = PageRank.UpdateRound();

      PhaseFinished(Phases.PageRankUpdateRound, updateResult);
    }

    public override void PageRankNormalizationHandler(PageRankNormalizationMessageReader request) {
      List<double> normalizationResult = PageRank.Normalization(request.Sum);

      PhaseFinished(Phases.PageRankNormalization, normalizationResult);
    }

    public override void PageRankRemoteUpdateHandler(PageRankRemoteUpdateMessageReader request) {
      PageRank.RemoteUpdate(request.Value, request.Target);
      var server = Global.CloudStorage[request.From];
      MultiLayerServer.MessagePassingExtension.PageRankRemoteUpdateAnswer(server);
    }

    public override void PageRankRemoteUpdateAnswerHandler() {
      PageRank.RemoteUpdateAnswer();
    }

  }
}
