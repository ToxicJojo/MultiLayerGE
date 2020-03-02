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

      double hubDelta = Double.MaxValue;
      double authDelta = Double.MaxValue;

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


    private double AuthUpdateRound () {
      foreach(var server in Global.CloudStorage) {
        using (var msg = new HITSUpdateMessageWriter(this.SeperateLayers)) {
          MultiLayerServer.MessagePassingExtension.HITSAuthUpdateRound(server, msg);
        }
      }

      List<List<double>> phaseResults = Proxy.WaitForPhaseResultsAsDouble(Phases.HITSAuthUpdateRound);

      double authSum = 0;

      foreach(List<double> result in phaseResults) {
        authSum += result[0];
      }

      return authSum;      
    }

    private double AuthNormalization (double hubSum) {
      foreach(var server in Global.CloudStorage) {
        using (var msg = new HITSNormalizationMessageWriter(hubSum)) {
          MultiLayerServer.MessagePassingExtension.HITSAuthNormalization(server, msg);
        }
      }

      List<List<double>> phaseResults = Proxy.WaitForPhaseResultsAsDouble(Phases.HITSAuthNormalization);

      double authDelta = 0;

      foreach(List<double> result in phaseResults) {
        authDelta += result[0];
      }

      return authDelta;
    }
  }
}
