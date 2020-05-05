using System;
using System.Collections.Generic;
using System.Threading;
using Trinity;
using MultiLayerProxy.Algorithms;
using MultiLayerProxy.Output;
using MultiLayerProxy.Util;
using MultiLayerLib;
using MultiLayerLib.MultiLayerProxy;

namespace MultiLayerProxy.Proxy {
  partial class MultiLayerProxyImpl: MultiLayerProxyBase {

    /// <summary>
    /// Used to count the servers that have finished a phase.
    /// </summary>
    private Dictionary<Phases, int> phaseFinishedCount = new Dictionary<Phases, int>();

    /// <summary>
    /// Used to lock the count for how many servers have finished a phase. This is neccessary as multiple servers might finish at the same time and we
    /// need to avoid lost updates.
    /// </summary>
    private readonly object phaseFinishedCountLock = new object();

    /// <summary>
    /// Collects the results servers send to the proxy when they finish a phase.
    /// </summary>
    private List<List<string>> phaseResults = new List<List<string>>();

    public MultiLayerProxyImpl () {
      RegisterPhases();
    }


    /// <summary>
    /// Fills the phaseFinishedCount dictonary with entries for evert phase defined in Phases.tsl
    /// </summary>
    private void RegisterPhases() {
      Array phases = Enum.GetValues(typeof(Phases));
      foreach(Phases phase in phases) {
        phaseFinishedCount.Add(phase, 0);
      }
    }

    /// <summary>
    /// Runs a given algorithm according to the given options.
    /// </summary>
    /// <param name="algorithm">The algorithm to run.</param>
    /// <param name="options">The options that should be applied to the algorithm.</param>
    private void RunAlgorithm (IAlgorithm algorithm, AlgorithmOptions options) {
      // If the algorithm is to be timed do a timed run.
      if (options.Timed) {
        TimeSpan ts = algorithm.TimedRun();

        // Format and display the TimeSpan value.
        string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);

        Console.WriteLine("Finished {0} in {1}", "Algorithm", elapsedTime);
      } else {
        algorithm.Run();
      }
    }

    /// <summary>
    /// Outputs the algorithms result according to the given options.
    /// The algorithm needs to be run before calling this.
    /// </summary>
    /// <param name="algorithm">The algorithm that has run and produced results.</param>
    /// <param name="options">The options that should be applied to the output.</param>
    private void OutputAlgorithmResult (IAlgorithm algorithm, OutputOptions options) {
      IOutputWriter outputWriter;

      // Create a new IOutputWriter that matches the type given in the options.
      if (options.OutputType == OutputType.Console) {
        outputWriter = new ConsoleOutputWriter(algorithm.Result);
        outputWriter.WriteOutput();
      } else if (options.OutputType == OutputType.CSV) {
        outputWriter = new CSVOutputWriter(algorithm.Result);
        outputWriter.WriteOutput();
      }

    }

    /// <summary>
    /// Handles PhaseFinishedRequests. This will updates the count for how many servers have finished a phase
    /// and add the phase result to the reults list.
    /// </summary>
    public override void PhaseFinishedHandler(PhaseFinishedMessageReader request) {
      // Lock the phaseFinishedCount to avoid lost updates.
      lock (phaseFinishedCountLock) {
        phaseResults.Add(request.Result);
        phaseFinishedCount[request.Phase]++;
      }
    }

    private void WaitForPhaseAnswers(Phases phase) {
      SpinWait wait = new SpinWait();

      while (phaseFinishedCount[phase] != Global.ServerCount) {
        wait.SpinOnce();
      }

      lock (phaseFinishedCountLock) {
        phaseFinishedCount[phase] = 0;
      }
    }

    /// <summary>
    /// Blocks until all servers have send a PhaseFinished message for a specified phase.
    /// </summary>
    /// <param name="phase">The phase to wait for.</param>
    public void WaitForPhase(Phases phase) {
      WaitForPhaseAnswers(phase);
      phaseResults.Clear();
    }

    public List<List<string>> WaitForPhaseResults(Phases phase) {
      WaitForPhaseAnswers(phase);
      
      // Create a copy of the results to return.
      List<List<string>> resultsCopy = new List<List<string>>(phaseResults);
      phaseResults.Clear();

      return resultsCopy;
    }

    /// <summary>
    /// Blocks until all servers have send a PhaseFinished message for a specified phase.
    /// </summary>
    /// <param name="phase">The phase to wait for.</param>
    /// <returns>A list of results for every server converted to longs.</returns>
    public List<List<long>> WaitForPhaseResultsAsLong(Phases phase) {
      List<List<string>> results = WaitForPhaseResults(phase);

      return ListHelper.ToLongList(results);
    }

    /// <summary>
    /// Blocks until all servers have send a PhaseFinished message for a specified phase.
    /// </summary>
    /// <param name="phase">The phase to wait for.</param>
    /// <returns>A list of results for every server converted to doubles.</returns>
    public List<List<double>> WaitForPhaseResultsAsDouble(Phases phase) {
      List<List<string>> results = WaitForPhaseResults(phase);

      return ListHelper.ToDoubleList(results);
    }

  }
}
