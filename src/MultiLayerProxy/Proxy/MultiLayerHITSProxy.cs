using MultiLayerProxy.Algorithms;
using MultiLayerLib;

namespace MultiLayerProxy.Proxy {

  partial class MultiLayerProxyImpl: MultiLayerProxyBase {

    public override void HITSProxyHandler(HITSProxyMessageReader request) {
      HITS hits = new HITS(this, request.InitialValue, request.Epsilon, request.SeperateLayers);

      RunAlgorithm(hits, request.AlgorithmOptions);
    }

    public override void HITSTopAuthoritiesProxyHandler(HITSTopNodesProxyMessageReader request) {
      HITSTopAuthorities topAuth = new HITSTopAuthorities(this, request.NumberOfTopNodes, request.SeperateLayers);

      RunAlgorithm(topAuth, request.AlgorithmOptions);
      OutputAlgorithmResult(topAuth, request.OutputOptions);
    }

    public override void HITSTopHubsProxyHandler(HITSTopNodesProxyMessageReader request) {
      HITSTopHubs topHubs = new HITSTopHubs(this, request.NumberOfTopNodes, request.SeperateLayers);

      RunAlgorithm(topHubs, request.AlgorithmOptions);
      OutputAlgorithmResult(topHubs, request.OutputOptions);
    }
    
  }
}
