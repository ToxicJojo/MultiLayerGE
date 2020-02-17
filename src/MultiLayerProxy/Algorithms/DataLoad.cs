using Trinity;
using MultiLayerProxy.Proxy;

namespace MultiLayerProxy.Algorithms {
  class DataLoad: Algorithm {

    private string ConfigFilePath { get; set; }

    public DataLoad (MultiLayerProxyImpl proxy, string configFilePath): base(proxy) {
      this.ConfigFilePath = configFilePath;
    }

    public override void Run() {
      foreach (var server in Global.CloudStorage) {
        using (var msg = new LoadGraphServerMessageWriter(this.ConfigFilePath)) {
          MultiLayerServer.MessagePassingExtension.LoadGraphServer(server, msg);
        }
      }

      Proxy.WaitForPhase(Phases.DataLoad);
    }
  }
}
