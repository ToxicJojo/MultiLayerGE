using Trinity;
using MultiLayerServer.Algorithms;
using System.Collections.Generic;

namespace MultiLayerServer.Server {
  public partial class MultiLayerServerImpl: MultiLayerServerBase {

   public override void HITSSetInitialValueHandler(HITSSEtInitialValueMessageReader request) {
     HITS.SetInitialValues(request.InitialValue);

     PhaseFinished(Phases.HITSInitialValues);
   }

   public override void HITSHubUpdateRoundHandler(HITSUpdateMessageReader request) {
     List<double> hubUpdateResult = HITS.HubUpdateRound(request.SeperateLayers);

     PhaseFinished(Phases.HITSHubUpdateRound, Util.ToStringList(hubUpdateResult));
   }

   public override void HITSHubNormalizationHandler(HITSNormalizationMessageReader request) {
     List<double> hubDelta = HITS.HubNormalization(request.Sum);

     PhaseFinished(Phases.HITSHubNormalization, Util.ToStringList(hubDelta));
   }

   public override void HITSHubRemoteUpdateHandler(HITSRemoteUpdateMessageReader request) {
     HITS.RemoteHubUpdate(request.Value, request.Target);

     MultiLayerServer.MessagePassingExtension.HITSHubRemoteUpdateAnswer(Global.CloudStorage[request.From]);
   }

   public override void HITSHubRemoteUpdateAnswerHandler() {
     HITS.RemoteHubUpdateAnswer();
   }
  }
}
