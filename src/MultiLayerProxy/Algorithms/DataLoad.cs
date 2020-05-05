using Trinity;
using MultiLayerProxy.Proxy;
using MultiLayerLib;
using MultiLayerLib.MultiLayerServer;

namespace MultiLayerProxy.Algorithms {

  /// <summary>
  /// An algorithm that loads data from the file system into GraphEngine.
  /// </summary>
  class DataLoad: Algorithm {

    /// <summary>
    /// The path to the config file for the data.
    /// </summary>
    private string ConfigFilePath { get; set; }

    public DataLoad (MultiLayerProxyImpl proxy, string configFilePath): base(proxy) {
      this.ConfigFilePath = configFilePath;
    }

    public override void Run() {
      foreach (var server in Global.CloudStorage) {
        using (var msg = new LoadGraphServerMessageWriter(this.ConfigFilePath)) {
          MessagePassingExtension.LoadGraphServer(server, msg);
        }
      }

      Proxy.WaitForPhase(Phases.DataLoad);
    }
  }
}
