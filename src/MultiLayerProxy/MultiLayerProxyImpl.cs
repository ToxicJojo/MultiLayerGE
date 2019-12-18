using System;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;
using Trinity;

namespace MultiLayerProxy {
    class MultiLayerProxyImpl: MultiGraphProxyBase {

      private string PHASE_DATA_LOAD = "phaseDataLoad";

      // Used to count how many servers have finished a certain phase.
      private Dictionary<string, int> phaseFinishedCount = new Dictionary<string, int>();
      // We need to lock the phaseFinishedCount Dictionary because multiple PhaseFinished messages can be recieved at the same time.
      private readonly object phaseFinishedCountLock = new object();

      public MultiLayerProxyImpl () {
        phaseFinishedCount[PHASE_DATA_LOAD] = 0;
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

      public override void PhaseFinishedHandler(PhaseFinishedMessageReader request) {
        lock (phaseFinishedCountLock) {
          phaseFinishedCount[request.Phase]++;
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
