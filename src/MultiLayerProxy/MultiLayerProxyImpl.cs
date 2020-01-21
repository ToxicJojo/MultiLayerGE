using System;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;
using Trinity;

namespace MultiLayerProxy {
    partial class MultiLayerProxyImpl: MultiGraphProxyBase {

      private string PHASE_DATA_LOAD = "phaseDataLoad";
      private string PHASE_NODE_COUNT = "phaseNodeCount";
      private string PHASE_EDGE_COUNT = "phaseEdgeCount";


      // Used to count how many servers have finished a certain phase.
      private Dictionary<string, int> phaseFinishedCount = new Dictionary<string, int>();
      // We need to lock the phaseFinishedCount Dictionary because multiple PhaseFinished messages can be recieved at the same time.
      private readonly object phaseFinishedCountLock = new object();
      // Collect all the results from different servers in this
      private List<List<double>> phaseResults = new List<List<double>>();

      public MultiLayerProxyImpl () {
        phaseFinishedCount[PHASE_DATA_LOAD] = 0;
        phaseFinishedCount[PHASE_NODE_COUNT] = 0;
        phaseFinishedCount[PHASE_EDGE_COUNT] = 0;
        phaseFinishedCount[PHASE_PAGE_RANK_INITIAL_VALUES] = 0;
        phaseFinishedCount[PHASE_PAGE_RANK_UPDATE] = 0;
        phaseFinishedCount[PHASE_PAGE_RANK_NORMALIZATION] = 0;
      }

      public override void LoadGraphHandler() {
        Console.WriteLine("Loading Graph");

        Stopwatch stopWatch = new Stopwatch();
        stopWatch.Start();

        foreach (var server in Global.CloudStorage) {
            MultiGraphServer.MessagePassingExtension.LoadGraphServer(server);
        }

        WaitForPhase(PHASE_DATA_LOAD);
        stopWatch.Stop();
        TimeSpan ts = stopWatch.Elapsed;

        // Format and display the TimeSpan value.
        string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);

        Console.WriteLine("Loaded Graph in " + elapsedTime);
      }

      public int[] GetNodeCount() {
        phaseResults.Clear();

        foreach(var server in Global.CloudStorage) {
          MultiGraphServer.MessagePassingExtension.GetNodeCount(server);
        }

        WaitForPhase(PHASE_NODE_COUNT);

        int[] nodeCount = new int[phaseResults[0].Count];
        
        foreach(List<double> result in phaseResults) {
          for (int i = 0; i < result.Count; i++) {
              nodeCount[i] +=(int) result[i];
          }
        }

        return nodeCount;
      }

      public int[] GetEdgeCount() {
        phaseResults.Clear();

        foreach(var server in Global.CloudStorage) {
          MultiGraphServer.MessagePassingExtension.GetEdgeCount(server);
        }

        WaitForPhase(PHASE_EDGE_COUNT);

        int[] edgeCount = new int[phaseResults[0].Count];

        foreach(List<double> result in phaseResults) {
          for(int i = 0; i < result.Count; i++) {
            edgeCount[i] += (int) result[i];
          }
        }

        return edgeCount;
      }

      public override void PhaseFinishedHandler(PhaseFinishedMessageReader request) {
        lock (phaseFinishedCountLock) {
          phaseFinishedCount[request.Phase]++;
          phaseResults.Add(request.Result);
        }
      }

      private void WaitForPhase(string phaseName) {
        SpinWait wait = new SpinWait();
        // Spin until the phseFinishedCount is eaul to the server count.
        while (phaseFinishedCount[phaseName] != Global.ServerCount) {
          wait.SpinOnce();
        }
        // Reset the count back to 0.
        lock (phaseFinishedCountLock) {
          phaseFinishedCount[phaseName] = 0;
        }
      }
    }
}
