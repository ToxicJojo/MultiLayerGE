using System;
using System.Collections.Generic;
using Trinity;
using MultiLayerProxy.Proxy;
using MultiLayerProxy.Util;
using MultiLayerLib;
using MultiLayerLib.MultiLayerServer;


namespace MultiLayerProxy.Algorithms {

  class HITS: Algorithm {
    private double InitialValue { get; set; }

    private double Epsilon { get; set; }

    private bool SeperateLayers { get; set; }

    public HITS (MultiLayerProxyImpl proxy, double initialValue, double epsilon, bool seperateLayers): base(proxy) {
      this.InitialValue = initialValue;
      this.Epsilon = epsilon; 
      this.SeperateLayers = seperateLayers;
      this.Name = "HITS";
    }

    public override void Run() {
      SetInitialValues();

      double hubDelta = Double.MaxValue;
      double authDelta = Double.MaxValue;

      // Keep doing updates until the change in hub and auth values is below epsilon.
      while (hubDelta > Epsilon || authDelta > Epsilon) {
        double authSum = AuthUpdateRound();
        authDelta = AuthNormalization(authSum);
        
        double hubSum = HubUpdateRound();
        hubDelta = HubNormalization(hubSum);

        Console.WriteLine("[HITS] AuthDelta: {0}  HubDelta: {1}", authDelta, hubDelta);
      }
    }


    private void SetInitialValues() {
      foreach(var server in Global.CloudStorage) {
        using(var msg = new HITSSEtInitialValueMessageWriter(this.InitialValue)) {
          MessagePassingExtension.HITSSetInitialValue(server, msg);
        }
      }

      Proxy.WaitForPhase(Phases.HITSInitialValues);
    }

    private double HubUpdateRound () {
      foreach(var server in Global.CloudStorage) {
        using (var msg = new HITSUpdateMessageWriter(this.SeperateLayers)) {
          MessagePassingExtension.HITSHubUpdateRound(server, msg);
        }
      }

      List<List<double>> phaseResults = Proxy.WaitForPhaseResultsAsDouble(Phases.HITSHubUpdateRound);

      double hubSum = ResultHelper.SumUpLayerResults(phaseResults, 0);

      return hubSum;
    }

    private double HubNormalization (double hubSum) {
      foreach(var server in Global.CloudStorage) {
        using (var msg = new HITSNormalizationMessageWriter(hubSum)) {
          MessagePassingExtension.HITSHubNormalization(server, msg);
        }
      }

      List<List<double>> phaseResults = Proxy.WaitForPhaseResultsAsDouble(Phases.HITSHubNormalization);

      double hubDelta = ResultHelper.SumUpLayerResults(phaseResults, 0);

      return hubDelta;
    }


    private double AuthUpdateRound () {
      foreach(var server in Global.CloudStorage) {
        using (var msg = new HITSUpdateMessageWriter(this.SeperateLayers)) {
          MessagePassingExtension.HITSAuthUpdateRound(server, msg);
        }
      }

      List<List<double>> phaseResults = Proxy.WaitForPhaseResultsAsDouble(Phases.HITSAuthUpdateRound);

      double authSum = ResultHelper.SumUpLayerResults(phaseResults, 0);

      return authSum;      
    }

    private double AuthNormalization (double hubSum) {
      foreach(var server in Global.CloudStorage) {
        using (var msg = new HITSNormalizationMessageWriter(hubSum)) {
          MessagePassingExtension.HITSAuthNormalization(server, msg);
        }
      }

      List<List<double>> phaseResults = Proxy.WaitForPhaseResultsAsDouble(Phases.HITSAuthNormalization);

      double authDelta = ResultHelper.SumUpLayerResults(phaseResults, 0);

      return authDelta;
    }
  }
}
