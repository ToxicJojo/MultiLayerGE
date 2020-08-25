using System;
using MultiLayerLib;

namespace MultiLayerServer.Loading {

  class JournalsLoader: IEdgeLoader {
    public Edge LoadEdge(string line)  {
      string[] fields = line.Split();

      long startId = long.Parse(fields[0]);
      int layer = int.Parse(fields[2]);
      long endId = long.Parse(fields[1]);
      float weight = 0;

      return new Edge(startId, layer, endId, layer, weight);
    }

    public long GetId (string line) {
      return long.Parse(line.Split()[0]);
    }
    public int GetLayer (string line) {
      return int.Parse(line.Split()[2]);
    }
  }
}
