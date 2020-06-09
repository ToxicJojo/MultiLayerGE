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

    private EdgeType EdgeType { get; set; }

    public DataLoad (MultiLayerProxyImpl proxy, string configFilePath, EdgeType edgeType): base(proxy) {
      this.ConfigFilePath = configFilePath;
      this.EdgeType = EdgeType;
      this.Name = "DataLoad";
    }

    public override void Run() {
      GraphConfig graphConfig = GraphConfig.LoadConfig(ConfigFilePath);

      Graph.Init();
      Graph.LoadLayers(graphConfig.LayersFilePath);

      foreach (var server in Global.CloudStorage) {
        using (var msg = new LoadGraphServerMessageWriter(this.ConfigFilePath, this.EdgeType)) {
          MessagePassingExtension.LoadGraphServer(server, msg);
        }
      }

      Proxy.WaitForPhase(Phases.DataLoad);
    }
  }
}
