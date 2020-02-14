using System;
using System.Collections.Generic;
using Trinity;
using MultiLayerProxy.Proxy;
using MultiLayerProxy.Output;

namespace MultiLayerProxy.Algorithms {

  class EdgeCount: Algorithm {

    public EdgeCount (MultiLayerProxyImpl proxy): base(proxy) {
      this.AlgorithmType = AlgorithmType.NodeCount;
    }

    public override void Run() {
      foreach(var server in Global.CloudStorage) {
        MultiLayerServer.MessagePassingExtension.GetEdgeCountServer(server);
      }

      List<List<long>> phaseResults =  Proxy.WaitForPhaseResultsAsLong(Phases.EdgeCount);

      long[] edgeCount = new long[phaseResults[0].Count];

      foreach(List<long> result in phaseResults) {
        for (int i = 0; i < result.Count; i++) {
            edgeCount[i] += (int) result[i];
        }
      }

      List<List<string>> output = new List<List<string>>();

      for (int i = 0; i < edgeCount.Length; i++) {
          List<string> outputRow = new List<string>();
          outputRow.Add("Layer " + (i + 1).ToString());
          outputRow.Add(edgeCount[i].ToString());
          output.Add(outputRow);
      }

      Result = new AlgorithmResult("EdgeCount", output);
    }
  }
}
