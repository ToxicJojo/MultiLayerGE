using System;
using System.Collections.Generic;
using Trinity;
using MultiLayerProxy.Proxy;

namespace MultiLayerProxy.Algorithms {

  class PageRank: Algorithm {


    private double InitialValue { get; set; }

    private double Epsilon { get; set; }

    public PageRank (MultiLayerProxyImpl proxy, double initialValue, double epsilon): base(proxy) {
      this.InitialValue = initialValue;
      this.Epsilon = epsilon; 
    }

    public override void Run() {
      SetInitialValues();

      double pageRankDelta = double.MaxValue;

      while (pageRankDelta > Epsilon) {
        double pageRankValueSum = UpdateRound();
        pageRankDelta = Normalization(pageRankValueSum);
        Console.WriteLine("[PageRank] UpdateRound finished with delta: {0}", pageRankDelta);
      }
    }

    private void SetInitialValues() {
      foreach(var server in Global.CloudStorage) {
        using (var msg = new PageRankSetInitialValuesMessageWriter(this.InitialValue)) {
          MultiLayerServer.MessagePassingExtension.PageRankSetInitialValues(server, msg);
        }
      }
      Proxy.WaitForPhase(Phases.PageRankInitialValues);
    }

    private double UpdateRound () {
      foreach(var server in Global.CloudStorage) {
        MultiLayerServer.MessagePassingExtension.PageRankUpdateRound(server);
      }

      List<List<double>> phaseResults = Proxy.WaitForPhaseResultsAsDouble(Phases.PageRankUpdateRound);

      double pageRankValueSum = 0;

      foreach(List<double> result in phaseResults) {
        pageRankValueSum += result[0];
      }

      return pageRankValueSum;
    }

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
