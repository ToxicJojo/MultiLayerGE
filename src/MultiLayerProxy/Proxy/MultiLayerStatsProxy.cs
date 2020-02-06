using MultiLayerProxy.Algorithms;

namespace MultiLayerProxy.Proxy {

  partial class MultiLayerProxyImpl: MultiLayerProxyBase {

    public override void GetNodeCountProxyHandler(GetNodeCountMessageReader request) {
      NodeCount nodeCount = new NodeCount(this);

      RunAlgorithm(nodeCount, request.Options);
    }
  }
}
