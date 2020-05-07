using MultiLayerProxy.Algorithms;
using MultiLayerLib;

namespace MultiLayerProxy.Proxy {
   partial class MultiLayerProxyImpl: MultiLayerProxyBase {

      public override void LoadGraphProxyHandler(LoadGraphProxyMessageReader request) {
         DataLoad dataLoad = new DataLoad(this, request.ConfigFilePath);
         
         RunAlgorithm(dataLoad, request.AlgorithmOptions);
      }
   }
}
