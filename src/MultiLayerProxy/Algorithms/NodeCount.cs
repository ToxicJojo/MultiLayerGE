using System;
using System.Collections.Generic;
using Trinity;
using MultiLayerProxy.Proxy;

namespace MultiLayerProxy.Algorithms {

  class NodeCount: Algorithm {

    public NodeCount (MultiLayerProxyImpl proxy): base(proxy) {
      this.AlgorithmType = AlgorithmType.NodeCount;
      Result = new List<double>();
    }

    public override void Run() {
      Result.Clear();

      foreach(var server in Global.CloudStorage) {
        MultiLayerServer.MessagePassingExtension.GetNodeCountServer(server);
      }


      List<List<double>> phaseResults =  Proxy.WaitForPhaseResults(Phases.NodeCount);

      double[] nodeCount = new double[phaseResults[0].Count];

      foreach(List<double> result in phaseResults) {
        for (int i = 0; i < result.Count; i++) {
            nodeCount[i] += result[i];
        }
      }

      Result = new List<double>(nodeCount);
    }
  }
}
