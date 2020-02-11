using System;
using System.Collections.Generic;
using Trinity;
using MultiLayerProxy.Proxy;
using MultiLayerProxy.Output;

namespace MultiLayerProxy.Algorithms {

  class NodeCount: Algorithm {

    public NodeCount (MultiLayerProxyImpl proxy): base(proxy) {
      this.AlgorithmType = AlgorithmType.NodeCount;
    }

    public override void Run() {
      foreach(var server in Global.CloudStorage) {
        MultiLayerServer.MessagePassingExtension.GetNodeCountServer(server);
      }


      List<List<double>> phaseResults =  Proxy.WaitForPhaseResults(Phases.NodeCount);

      int[] nodeCount = new int[phaseResults[0].Count];

      foreach(List<double> result in phaseResults) {
        for (int i = 0; i < result.Count; i++) {
            nodeCount[i] += (int) result[i];
        }
      }

      List<List<string>> output = new List<List<string>>();

      for (int i = 0; i < nodeCount.Length; i++) {
          List<string> outputRow = new List<string>();
          outputRow.Add("Layer " + (i + 1).ToString());
          outputRow.Add(nodeCount[i].ToString());
          output.Add(outputRow);
      }

      Result = new AlgorithmResult("NodeCount", output);
    }
  }
}
