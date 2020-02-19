using System;
using System.Collections.Generic;
using Trinity;
using MultiLayerProxy.Proxy;

namespace MultiLayerProxy.Algorithms {

  class PageRank: Algorithm {

    private double InitialValue { get; set; }

    private double Epsilon { get; set; }

    private bool SeperateLayers { get; set; }

    public PageRank (MultiLayerProxyImpl proxy, double initialValue, double epsilon, bool seperateLayers): base(proxy) {
      this.InitialValue = initialValue;
      this.Epsilon = epsilon; 
      this.SeperateLayers = seperateLayers;
    }

    public override void Run() {
      SetInitialValues();

      double pageRankDelta = double.MaxValue;

      // Continue to do updates until the delta between the old values and the new ones is smaller than Epsilon.
      while (pageRankDelta > Epsilon) {
        double pageRankValueSum = UpdateRound();
        pageRankDelta = Normalization(pageRankValueSum);
        Console.WriteLine("[PageRank] UpdateRound finished with delta: {0}", pageRankDelta);
      }
    }

    /// <summary>
    /// Sets the initial values for all nodes.
    /// </summary>
    private void SetInitialValues() {
      foreach(var server in Global.CloudStorage) {
        using (var msg = new PageRankSetInitialValuesMessageWriter(this.InitialValue)) {
          MultiLayerServer.MessagePassingExtension.PageRankSetInitialValues(server, msg);
        }
      }
      Proxy.WaitForPhase(Phases.PageRankInitialValues);
    }

    /// <summary>
    /// Does one update round for all nodes.
    /// </summary>
    /// <returns>The sum of the new values.</returns>
    private double UpdateRound () {
      foreach(var server in Global.CloudStorage) {
        using (var msg = new PageRankUpdateMessageWriter(this.SeperateLayers)) {
          MultiLayerServer.MessagePassingExtension.PageRankUpdateRound(server, msg);
        }
      }

      List<List<double>> phaseResults = Proxy.WaitForPhaseResultsAsDouble(Phases.PageRankUpdateRound);

      double pageRankValueSum = 0;

      foreach(List<double> result in phaseResults) {
        pageRankValueSum += result[0];
      }

      return pageRankValueSum;
    }

    /// <summary>
    /// Normalizes the pagerank scores for all nodes based on the sum of all values.
    /// </summary>
    /// <param name="pageRankValueSum">The sum of all pagerank values.</param>
    /// <returns>The delta between the old and new values.</returns>
    private double Normalization (double pageRankValueSum) {
      foreach(var server in Global.CloudStorage) {
        using (var msg = new PageRankNormalizationMessageWriter(pageRankValueSum)) {
          MultiLayerServer.MessagePassingExtension.PageRankNormalization(server, msg);
        }
      }

      List<List<double>> phaseResults = Proxy.WaitForPhaseResultsAsDouble(Phases.PageRankNormalization);
      double pageRankDelta = 0;
      foreach(List<double> result in phaseResults) {
        pageRankDelta += result[0];
      }

      return pageRankDelta;
    }
  }
}
