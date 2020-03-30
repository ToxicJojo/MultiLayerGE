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

    public override void HITSAuthUpdateRoundHandler(HITSUpdateMessageReader request) {
      List<double> authUpdateResult = HITS.AuthUpdateRound(request.SeperateLayers);

      PhaseFinished(Phases.HITSAuthUpdateRound, Util.ToStringList(authUpdateResult));
    }

    public override void HITSAuthRemoteUpdateHandler(HITSRemoteUpdateMessageReader request) {
     HITS.RemoteAuthUpdate(request.Value, request.Target);

     MultiLayerServer.MessagePassingExtension.HITSAuthRemoteUpdateAnswer(Global.CloudStorage[request.From]);
    }

    public override void HITSAuthRemoteUpdateAnswerHandler() {
      HITS.RemoteAuthUpdateAnswer();
    }

    public override void HITSRemoteBulkUpdateHandler(HITSRemoteBulkUpdateMessageReader request) {
      HITS.RemoteBulkAuthUpdate(request.Updates);

      MultiLayerServer.MessagePassingExtension.HITSAuthRemoteUpdateAnswer(Global.CloudStorage[request.From]);
    }

    public override void HITSAuthNormalizationHandler(HITSNormalizationMessageReader request) {
      List<double> authDelta = HITS.AuthNormalization(request.Sum);

      PhaseFinished(Phases.HITSAuthNormalization, Util.ToStringList(authDelta));
    }

    public override void HITSTopAuthoritiesServerHandler(HITSTopNodesServerMessageReader request) {
      List<long> topAuthorities = HITS.TopAuthorities(request.NumberOfTopNodes, request.SeperateLayers);

      PhaseFinished(Phases.HITSTopAuthorities, Util.ToStringList(topAuthorities));
    }

    public override void HITSTopHubsServerHandler(HITSTopNodesServerMessageReader request) {
      List<long> topHubs = HITS.TopHubs(request.NumberOfTopNodes, request.SeperateLayers);

      PhaseFinished(Phases.HITSTopHubs, Util.ToStringList(topHubs));
    }


    public override void HITSGetBulkHubValuesHandler(HITSGetBulkHubValuesMessageReader request) {
      List<HubValuePair> values = HITS.GetHubValues(request.Ids);

      using (var msg = new HITSGetBulkHubValuesResponseWriter(values)) {
        MultiLayerServer.MessagePassingExtension.HITSGetBulkHubValuesAnswer(Global.CloudStorage[request.From], msg);
      }
    }

    public override void HITSGetBulkHubValuesAnswerHandler(HITSGetBulkHubValuesResponseReader request) {
      HITS.AddRemoteAuthScores(request.Values);
    }

  }
}
