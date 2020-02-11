using System;
using System.Collections.Generic;
using System.Threading;
using Trinity;
using MultiLayerProxy.Algorithms;
using MultiLayerProxy.Output;

namespace MultiLayerProxy.Proxy {
  partial class MultiLayerProxyImpl: MultiLayerProxyBase {
    private Dictionary<Phases, int> phaseFinishedCount = new Dictionary<Phases, int>();

    private readonly object phaseFinishedCountLock = new object();

    private List<List<double>> phaseResults = new List<List<double>>();

    public MultiLayerProxyImpl () {
      RegisterPhases();
    }


    private void RunAlgorithm (IAlgorithm algorithm, AlgorithmOptions options) {
      phaseResults.Clear();

      if (options.Timed) {
        algorithm.TimedRun();
      } else {
        algorithm.Run();
      }
    }

    private void OutputAlgorithmResult (IAlgorithm algorithm, OutputOptions options) {
      IOutputWriter outputWriter;

      if (options.OutputType == OutputType.Console) {
        outputWriter = new ConsoleOutputWriter(algorithm.Result);
      } else {
        outputWriter = new ConsoleOutputWriter(algorithm.Result);
      }

      outputWriter.WriteOutput();
    }

    public override void PhaseFinishedHandler(PhaseFinishedMessageReader request) {
      lock (phaseFinishedCountLock) {
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
