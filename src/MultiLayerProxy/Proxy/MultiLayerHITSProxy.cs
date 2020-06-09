using MultiLayerProxy.Algorithms;
using MultiLayerLib;

namespace MultiLayerProxy.Proxy {

  partial class MultiLayerProxyImpl: MultiLayerProxyBase {

    public override void HITSProxyHandler(HITSProxyMessageReader request) {
      HITS hits = new HITS(this, request.InitialValue, request.Epsilon, request.SeperateLayers);

      RunAlgorithm(hits, request.AlgorithmOptions);
    }

    public override void HITSTopAuthoritiesProxyHandler(HITSTopNodesProxyMessageReader request, AlgorithmResultWriter response) {
      HITSTopAuthorities topAuth = new HITSTopAuthorities(this, request.NumberOfTopNodes, request.SeperateLayers);

      RunAlgorithm(topAuth, request.AlgorithmOptions);
      OutputAlgorithmResult(topAuth, request.OutputOptions, response);
    }

    public override void HITSTopHubsProxyHandler(HITSTopNodesProxyMessageReader request, AlgorithmResultWriter response) {
      HITSTopHubs topHubs = new HITSTopHubs(this, request.NumberOfTopNodes, request.SeperateLayers);

      RunAlgorithm(topHubs, request.AlgorithmOptions);
      OutputAlgorithmResult(topHubs, request.OutputOptions, response);
    }
    
  }
}
