using System;
using System.Collections.Generic;
using System.Threading;
using Trinity;
using MultiLayerProxy.Algorithms;

namespace MultiLayerProxy.Proxy {
  partial class MultiLayerProxyImpl: MultiLayerProxyBase {
    private Dictionary<Phases, int> phaseFinishedCount = new Dictionary<Phases, int>();

    private readonly object phaseFinishedCountLock = new object();

    private List<List<double>> phaseResults = new List<List<double>>();

    public MultiLayerProxyImpl () {
      RegisterPhases();
    }


    private void RunAlgorithm (IAlgorithm algorithm, AlgorithmOptions options) {
      if (options.Timed) {
        algorithm.TimedRun();
      } else {
        algorithm.Run();
      }
    }

    public override void PhaseFinishedHandler(PhaseFinishedMessageReader request) {
      lock (phaseFinishedCountLock) {

        Console.WriteLine("Phase finished {0}", request.Phase.ToString());

        foreach(double count in request.Result) {
          Console.WriteLine(count);
        }

        phaseResults.Add(request.Result);
        phaseFinishedCount[request.Phase]++;
      }
    }

    private void RegisterPhases() {
      Array phases = Enum.GetValues(typeof(Phases));
      foreach(Phases phase in phases) {
        phaseFinishedCount.Add(phase, 0);
      }
    }

    public void WaitForPhase(Phases phase) {
      SpinWait wait = new SpinWait();

      while (phaseFinishedCount[phase] != Global.ServerCount) {
        wait.SpinOnce();
      }

      lock (phaseFinishedCountLock) {
        phaseFinishedCount[phase] = 0;
      }
    }

    public List<List<double>> WaitForPhaseResults(Phases phase) {
      WaitForPhase(phase);

      List<List<double>> resultsCopy = new List<List<double>>(phaseResults);
      phaseResults.Clear();

      return resultsCopy;
    }
  }
}
