using System;

namespace MultiLayerServer.Loading {
  interface IEdgeLoader {

    Edge LoadEdge(string line);

    long GetId(string line);

    int GetLayer(string line);
  }
}
