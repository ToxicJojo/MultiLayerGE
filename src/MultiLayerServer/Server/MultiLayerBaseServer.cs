using System;
using System.Collections.Generic;
using Trinity;
using MultiLayerLib;
using MultiLayerLib.MultiLayerProxy;

namespace MultiLayerServer.Server {

  public partial class MultiLayerServerImpl: MultiLayerServerBase {

    /// <summary>
    /// Sends a message to the proxy indicating a phase of an algorithm has finished.
    /// </summary>
    /// <param name="phase">The phase that has been finished.</param>
    private void PhaseFinished(Phases phase) {
      PhaseFinished(phase, new List<string>());
    }

    /// <summary>
    /// Sends a message to the proxy indicating a phase of an algorithm has finished and
    /// what the result of that phase has been.
    /// </summary>
    /// <param name="phase">The phase that has been finished.</param>
    /// <param name="result">A list of results for the phase. </param>
    private void PhaseFinished(Phases phase, List<string> result) {
      using (var msg = new PhaseFinishedMessageWriter(result, phase)) {
        MessagePassingExtension.PhaseFinished(Global.CloudStorage.ProxyList[0], msg);
      }
    }
  }
}
