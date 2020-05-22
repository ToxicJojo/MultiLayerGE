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
          break;
        case EdgeType.MultiLayerDirectedWeightedEdge:
          graphLoader = new GraphLoader(new MultiLayerDirectedWeightedEdgeLoader());
          break;
         case EdgeType.Journals:
          graphLoader = new GraphLoader(new JournalsLoader());
          break;       
        default:
          Console.WriteLine("No edgeloader avaiable for the requested edge type.");
          return;
      }

      graphLoader.LoadGraph(configFilePath);
    }
  }
}
