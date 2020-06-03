using MultiLayerProxy.Algorithms;
using MultiLayerLib;
using MultiLayerLib.Output;

namespace MultiLayerProxy.Proxy {

  partial class MultiLayerProxyImpl: MultiLayerProxyBase {

    public override void GetNodeCountProxyHandler(StandardAlgorithmMessageReader request, AlgorithmResultWriter response) {
      NodeCount nodeCount = new NodeCount(this);
      RunAlgorithm(nodeCount, request.AlgorithmOptions);
      OutputAlgorithmResult(nodeCount, request.OutputOptions, response);
    }

    public override void GetEdgeCountProxyHandler(StandardAlgorithmMessageReader request, AlgorithmResultWriter response) {
      EdgeCount edgeCount = new EdgeCount(this);

      RunAlgorithm(edgeCount, request.AlgorithmOptions);
      OutputAlgorithmResult(edgeCount, request.OutputOptions, response);
    }

    public override void GetGraphDensityProxyHandler(StandardAlgorithmMessageReader request, AlgorithmResultWriter response) {
      GraphDensity graphDensity = new GraphDensity(this);

      RunAlgorithm(graphDensity, request.AlgorithmOptions);
      OutputAlgorithmResult(graphDensity, request.OutputOptions, response);
    }
  }
}
