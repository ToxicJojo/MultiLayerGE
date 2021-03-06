using MultiLayerProxy.Algorithms;
using MultiLayerLib;

namespace MultiLayerProxy.Proxy {

  partial class MultiLayerProxyImpl: MultiLayerProxyBase {

    public override void EgoNetworkProxyHandler(EgoNetworkMessageProxyReader request, AlgorithmResultWriter response) {
      EgoNetwork egoNetwork = new EgoNetwork(this, request.Id, request.Layer, request.SeperateLayers);

      RunAlgorithm(egoNetwork, request.AlgorithmOptions);

      OutputAlgorithmResult(egoNetwork, request.OutputOptions, response);
    }
  }
}
