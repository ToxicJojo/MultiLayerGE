using MultiLayerProxy.Algorithms;
using MultiLayerProxy.Output;

namespace MultiLayerProxy.Proxy {

  partial class MultiLayerProxyImpl: MultiLayerProxyBase {

    public override void EgoNetworkProxyHandler(EgoNetworkMessageProxyReader request) {
      EgoNetwork egoNetwork = new EgoNetwork(this, request.Id, request.Layer, request.SeperateLayers);

      RunAlgorithm(egoNetwork, request.AlgorithmOptions);
      OutputAlgorithmResult(egoNetwork, request.OutputOptions);
    }
  }
}
