using MultiLayerProxy.Algorithms;

namespace MultiLayerProxy.Proxy {

  partial class MultiLayerProxyImpl: MultiLayerProxyBase {

    public override void PageRankProxyHandler(PageRankProxyMessageReader request) {
      PageRank pageRank = new PageRank(this, request.InitialValue, request.Epsilon);

      RunAlgorithm(pageRank, request.AlgorithmOptions);
    }
  }
}
