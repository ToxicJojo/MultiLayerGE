using System;
using System.Collections.Generic;
using MultiLayerServer.Algorithms;

namespace MultiLayerServer.Server {
  public partial class MultiLayerServerImpl: MultiLayerServerBase {

    public override void EgoNetworkServerHandler(EgoNetworkMessageServerReader request) {
      List<long> incomingNetwork = EgoNetwork.GetIncomingNetwork(request.Id, request.Layer, request.SeperateLayers);

      PhaseFinished(Phases.EgoNetwork, Util.ToStringList(incomingNetwork));
    }
  }
}
