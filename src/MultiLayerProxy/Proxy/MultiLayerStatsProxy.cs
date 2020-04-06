using MultiLayerProxy.Algorithms;
using MultiLayerProxy.Output;

namespace MultiLayerProxy.Proxy {

  partial class MultiLayerProxyImpl: MultiLayerProxyBase {

    public override void GetNodeCountProxyHandler(StandardAlgorithmMessageReader request) {
      NodeCount nodeCount = new NodeCount(this);

      RunAlgorithm(nodeCount, request.AlgorithmOptions);
      OutputAlgorithmResult(nodeCount, request.OutputOptions);
    }

    public override void GetEdgeCountProxyHandler(StandardAlgorithmMessageReader request) {
      EdgeCount edgeCount = new EdgeCount(this);

      RunAlgorithm(edgeCount, request.AlgorithmOptions);
      OutputAlgorithmResult(edgeCount, request.OutputOptions);
    }

    public override void GetGraphDensityProxyHandler(StandardAlgorithmMessageReader request) {
      GraphDensity graphDensity = new GraphDensity(this);

      RunAlgorithm(graphDensity, request.AlgorithmOptions);
      OutputAlgorithmResult(graphDensity, request.OutputOptions);
    }
  }
}
