using MultiLayerProxy.Algorithms;
using MultiLayerLib;

namespace MultiLayerProxy.Proxy {

  partial class MultiLayerProxyImpl: MultiLayerProxyBase {

    public override void DegreeProxyHandler(DegreeProxyMessageReader request) {
      Degree degree = new Degree(this, request.SeperateLayers);

      RunAlgorithm(degree, request.AlgorithmOptions);
    }
  }
}
