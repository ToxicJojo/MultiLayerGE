using System;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;
using Trinity;

namespace MultiLayerProxy {
    partial class MultiLayerProxyImpl: MultiGraphProxyBase {
      
      private string PHASE_PAGE_RANK_INITIAL_VALUES = "phasePageRankInitialValues";
      private string PHASE_PAGE_RANK_UPDATE = "phasePageRankUpdate";
      private string PHASE_PAGE_RANK_NORMALIZATION = "phasePageRankNormalization";


      private double epsilon = 0.5;

      public void RunPageRank () {

        Console.WriteLine("[PageRank] Started setting initial values.");
        SetInitialValues(1);
        Console.WriteLine("[PageRank] Finished setting initial values.");

        Console.WriteLine("[PageRank] Started update.");
        double pageRankDelta = double.MaxValue;
        while (pageRankDelta > epsilon) {
          double pageRankValueSum = PageRankUpdate();
          pageRankDelta = PageRankNormalization(pageRankValueSum);
        }
        Console.WriteLine("[PageRank] Finished update.");
      }


      private void SetInitialValues(double value) {
        
        foreach(var server in Global.CloudStorage) {
          SetInitialValueMessageWriter msg = new SetInitialValueMessageWriter(value);
          MultiGraphServer.MessagePassingExtension.SetInitialValue(server, msg);
        }

        WaitForPhase(PHASE_PAGE_RANK_INITIAL_VALUES);
      }


      private double PageRankUpdate() {
        phaseResults.Clear();
        foreach(var server in Global.CloudStorage) {
          MultiGraphServer.MessagePassingExtension.PageRankUpdate(server);
        }

        WaitForPhase(PHASE_PAGE_RANK_UPDATE);

        double pageRankValueSum = 0;
        foreach(List<double> result in phaseResults) {
          pageRankValueSum += result[0];
        }

        Console.WriteLine("[Page Rank] Value Sum {0}", pageRankValueSum);

        return pageRankValueSum;
      }

      private double PageRankNormalization (double pageRankValueSum) {
        phaseResults.Clear();

        foreach (var server in Global.CloudStorage) {
            PageRankNormalizationMessageWriter msg = new PageRankNormalizationMessageWriter(pageRankValueSum);
            MultiGraphServer.MessagePassingExtension.PageRankNormalization(server, msg);
        }

        WaitForPhase(PHASE_PAGE_RANK_NORMALIZATION);

        double pageRankDelta = 0;
        foreach(List<double> result in phaseResults) {
          pageRankDelta += result[0];
        }

        Console.WriteLine("[Page Rank] Delta: {0}", pageRankDelta);
        return pageRankDelta;
      }
    }
}
