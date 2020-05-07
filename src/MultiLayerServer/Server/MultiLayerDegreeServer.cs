using System;
using System.Collections.Generic;
using MultiLayerServer.Algorithms;
using Trinity;
using MultiLayerLib;
using MultiLayerLib.MultiLayerServer;

namespace MultiLayerServer.Server {
  public partial class MultiLayerServerImpl: MultiLayerServerBase {

    public override void GetOutDegreeServerHandler(DegreeServerMessageReader request) {
      Degree.GetOutDegree(request.SeperateLayers);

      PhaseFinished(Phases.DegreeOut);
    }

    public override void GetInDegreeServerHandler(DegreeServerMessageReader request) {
      Degree.GetInDegree(request.SeperateLayers);

      PhaseFinished(Phases.DegreeIn);
    }

    public override void DegreeBulkUpdateHandler(RemoteBulkUpdateMessageReader request) {
      Degree.RemoteBulkUpdate(request.Values);

      var server = Global.CloudStorage[request.From];
      MessagePassingExtension.DegreeBulkUpdateAnswer(server);
    }

    public override void DegreeBulkUpdateAnswerHandler() {
      Degree.RemoteUpdateAnswer();
    }

    public override void DegreeGetTotalHandler() {
      Degree.GetTotalDegree();

      PhaseFinished(Phases.DegreeTotal);
    }
  }
}
