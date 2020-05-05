using System;
using System.Collections.Generic;
using MultiLayerServer.Algorithms;
using MultiLayerLib;

namespace MultiLayerServer.Server {
  public partial class MultiLayerServerImpl: MultiLayerServerBase {

    public override void LoadGraphServerHandler(LoadGraphServerMessageReader request) {
      DataLoad.LoadData(request.ConfigFilePath);
      
      PhaseFinished(Phases.DataLoad);
    }
  }
}
