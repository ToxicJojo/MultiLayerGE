using System;
using System.Collections.Generic;
using Trinity;
using MultiLayerProxy.Proxy;


namespace MultiLayerProxy.Algorithms {

  class HITS: Algorithm {
    private double InitialValue { get; set; }

    private double Epsilon { get; set; }

    private bool SeperateLayers { get; set; }

    public HITS (MultiLayerProxyImpl proxy, double initialValue, double epsilon, bool seperateLayers): base(proxy) {
      this.InitialValue = initialValue;
      this.Epsilon = epsilon; 
      this.SeperateLayers = seperateLayers;
    }

    public override void Run() {
      Console.WriteLine("Runnig HITS");

      SetInitialValues();
      double hubSum = HubUpdateRound();
      double hubDelta = HubNormalization(hubSum);

      Console.WriteLine("HubUpdate done with sum {0} and delta {1}", hubSum, hubDelta);
    }


    private void SetInitialValues() {
      foreach(var server in Global.CloudStorage) {
        using(var msg = new HITSSEtInitialValueMessageWriter(this.InitialValue)) {
          MultiLayerServer.MessagePassingExtension.HITSSetInitialValue(server, msg);
        }
      }

      Proxy.WaitForPhase(Phases.HITSInitialValues);
    }

    private double HubUpdateRound () {
      foreach(var server in Global.CloudStorage) {
        using (var msg = new HITSUpdateMessageWriter(this.SeperateLayers)) {
          MultiLayerServer.MessagePassingExtension.HITSHubUpdateRound(server, msg);
        }
      }

      List<List<double>> phaseResults = Proxy.WaitForPhaseResultsAsDouble(Phases.HITSHubUpdateRound);

      double hubSum = 0;

      foreach(List<double> result in phaseResults) {
        hubSum += result[0];
      }

      return hubSum;
    }

    private double HubNormalization (double hubSum) {
      foreach(var server in Global.CloudStorage) {
        using (var msg = new HITSNormalizationMessageWriter(hubSum)) {
          MultiLayerServer.MessagePassingExtension.HITSHubNormalization(server, msg);
        }
      }

      List<List<double>> phaseResults = Proxy.WaitForPhaseResultsAsDouble(Phases.HITSHubNormalization);

      double hubDelta = 0;

      foreach(List<double> result in phaseResults) {
        hubDelta += result[0];
      }

      return hubDelta;
    }
  }
}
