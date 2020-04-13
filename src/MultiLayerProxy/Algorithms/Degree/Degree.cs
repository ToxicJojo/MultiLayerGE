using System.Collections.Generic;
using Trinity;
using MultiLayerProxy.Proxy;
using MultiLayerProxy.Output;

namespace MultiLayerProxy.Algorithms {
  /// <summary>
  /// Counts all the edges in the graph. The edges are counted for each layer seperatly.
  /// </summary>
  class Degree: Algorithm {

    private bool SeperateLayers { get; set; }

    public Degree (MultiLayerProxyImpl proxy, bool seperateLayers): base(proxy) {
      SeperateLayers = seperateLayers;
    }

    public override void Run() {
      foreach(var server in Global.CloudStorage) {
        using (var msg = new DegreeServerMessageWriter(SeperateLayers)) {
          MultiLayerServer.MessagePassingExtension.GetOutDegreeServer(server, msg);
        }
      }

      Proxy.WaitForPhase(Phases.DegreeOut);

      foreach(var server in Global.CloudStorage) {
        using (var msg = new DegreeServerMessageWriter(SeperateLayers)) {
          MultiLayerServer.MessagePassingExtension.GetInDegreeServer(server,  msg);
        }
      }

      Proxy.WaitForPhase(Phases.DegreeIn);

      foreach(var server in Global.CloudStorage) {
        MultiLayerServer.MessagePassingExtension.DegreeGetTotal(server);
      }

      Proxy.WaitForPhase(Phases.DegreeTotal);
    }
  }
}
