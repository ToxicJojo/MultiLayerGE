using MultiLayerProxy.Algorithms;
using MultiLayerLib;

namespace MultiLayerProxy.Proxy {

  partial class MultiLayerProxyImpl: MultiLayerProxyBase {

    public override void PageRankProxyHandler(PageRankProxyMessageReader request) {
      PageRank pageRank = new PageRank(this, request.InitialValue, request.Epsilon, request.SeperateLayers);

      RunAlgorithm(pageRank, request.AlgorithmOptions);
    }

    public override void PageRankTopNodesProxyHandler(PageRankTopNodesProxyMessageReader request, AlgorithmResultWriter response) {
      PageRankTopNodes pageRankTopNodes = new PageRankTopNodes(this, request.NumberOfTopNodes, request.SeperateLayers);

      RunAlgorithm(pageRankTopNodes, request.AlgorithmOptions);
      OutputAlgorithmResult(pageRankTopNodes, request.OutputOptions, response);
    }
  }
}
