using System;
using System.Collections.Generic;
using MultiLayerServer.Algorithms;

namespace MultiLayerServer.Server {
  public partial class MultiLayerServerImpl: MultiLayerServerBase {

    public override void GetNodeCountServerHandler() {
      List<double> result = Stats.GetNodeCount();

      PhaseFinished(Phases.NodeCount, result);
    }

    public override void GetEdgeCountServerHandler() {
      List<double> result = Stats.GetEdgeCount();

      PhaseFinished(Phases.EdgeCount, result);
    }
  }
}
