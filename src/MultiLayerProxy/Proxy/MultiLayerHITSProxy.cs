using System;
using MultiLayerProxy.Algorithms;

namespace MultiLayerProxy.Proxy {

  partial class MultiLayerProxyImpl: MultiLayerProxyBase {

    public override void HITSProxyHandler(HITSProxyMessageReader request) {
      HITS hits = new HITS(this, request.InitialValue, request.Epsilon, request.SeperateLayers);

      RunAlgorithm(hits, request.AlgorithmOptions);
    }
    
  }
}
