using System;
using System.Collections.Generic;
using MultiLayerServer.Algorithms;

namespace MultiLayerServer.Server {
  public partial class MultiLayerServerImpl: MultiLayerServerBase {

    public override void GetNodeCountServerHandler() {
      List<long> result = Stats.GetNodeCount();

      PhaseFinished(Phases.NodeCount, Util.ToStringList(result));
    }

    public override void GetEdgeCountServerHandler() {
      List<long> result = Stats.GetEdgeCount();

      PhaseFinished(Phases.EdgeCount, Util.ToStringList(result));
    }
  }
}
