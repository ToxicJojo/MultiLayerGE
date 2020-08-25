using System;
using MultiLayerServer.Loading;
using MultiLayerLib;

namespace MultiLayerServer.Algorithms {

  class DataLoad {

    public static void LoadData (string configFilePath, EdgeType edgeType) {
      GraphLoader graphLoader;
      switch(edgeType) {
        case EdgeType.MultilayerDirectedEdge:
          graphLoader = new GraphLoader(new MultiLayerDirectedEdgeLoader());
          Console.WriteLine("Using MultilayerDirectedEdge Loader");
          break;
        case EdgeType.MultiLayerDirectedWeightedEdge:
          graphLoader = new GraphLoader(new MultiLayerDirectedWeightedEdgeLoader());
          Console.WriteLine("Using MultiLayerDirectedWeightedEdge Loader");
          break;
         case EdgeType.Journals:
          graphLoader = new GraphLoader(new JournalsLoader());
          Console.WriteLine("Using Journals Loader");
          break;       
        default:
          Console.WriteLine("No edgeloader avaiable for the requested edge type.");
          return;
      }

      graphLoader.LoadGraph(configFilePath);
    }
  }
}
