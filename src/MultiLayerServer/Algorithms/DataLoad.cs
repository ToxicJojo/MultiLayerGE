using MultiLayerServer.Loading;

namespace MultiLayerServer.Algorithms {

  class DataLoad {

    public static void LoadData (string configFilePath) {
      GraphLoader graphLoader  = new GraphLoader(new MultiLayerDirectedWeightedEdgeLoader());
      //GraphLoader graphLoader  = new GraphLoader(new JournalsLoader());
      graphLoader.LoadGraph(configFilePath);
    }
  }
}
