using System.Collections.Generic;
using Trinity;
using MultiLayerProxy.Proxy;
using MultiLayerLib;
using MultiLayerLib.MultiLayerServer;

namespace MultiLayerProxy.Algorithms {
  /// <summary>
  /// Counts all the edges in the graph. The edges are counted for each layer seperatly.
  /// </summary>
  class Degree: Algorithm {

    private bool SeperateLayers { get; set; }

    public Degree (MultiLayerProxyImpl proxy, bool seperateLayers): base(proxy) {
      SeperateLayers = seperateLayers;
      this.Name = "Degree";
    }

    public override void Run() {
      foreach(var server in Global.CloudStorage) {
        using (var msg = new DegreeServerMessageWriter(SeperateLayers)) {
          MessagePassingExtension.GetOutDegreeServer(server, msg);
        }
      }

      Proxy.WaitForPhase(Phases.DegreeOut);

      foreach(var server in Global.CloudStorage) {
        using (var msg = new DegreeServerMessageWriter(SeperateLayers)) {
          MessagePassingExtension.GetInDegreeServer(server,  msg);
        }
      }

      Proxy.WaitForPhase(Phases.DegreeIn);

      foreach(var server in Global.CloudStorage) {
        MessagePassingExtension.DegreeGetTotal(server);
      }

      Proxy.WaitForPhase(Phases.DegreeTotal);
    }
  }
}
