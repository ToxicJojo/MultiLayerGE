using System.Collections.Generic;
using Trinity;
using MultiLayerProxy.Proxy;
using MultiLayerProxy.Output;
using MultiLayerLib;
using MultiLayerLib.MultiLayerServer;

namespace MultiLayerProxy.Algorithms {

  class NodeCount: Algorithm {

    public NodeCount (MultiLayerProxyImpl proxy): base(proxy) {
    }

    public override void Run() {
      foreach(var server in Global.CloudStorage) {
        MessagePassingExtension.GetNodeCountServer(server);
      }

      List<List<long>> phaseResults =  Proxy.WaitForPhaseResultsAsLong(Phases.NodeCount);
      long[] nodeCount = new long[phaseResults[0].Count];

      // Sum up the results from all the servers.
      foreach(List<long> result in phaseResults) {
        for (int i = 0; i < result.Count; i++) {
            nodeCount[i] += result[i];
        }
      }

      WriteOutput(nodeCount);
    }

    private void WriteOutput(long[] nodeCount) {
      List<List<string>> output = new List<List<string>>();
      long totalNodeCount = 0;

      for (int i = 0; i < nodeCount.Length; i++) {
          List<string> outputRow = new List<string>();
          outputRow.Add("Layer " + (i + 1).ToString());
          outputRow.Add(nodeCount[i].ToString());
          output.Add(outputRow);

          totalNodeCount += nodeCount[i];
      }

      List<string> totalOutputRow = new List<string>();
      totalOutputRow.Add("Total");
      totalOutputRow.Add(totalNodeCount.ToString());
      output.Add(totalOutputRow);

      Result = new AlgorithmResult("NodeCount", output);
    }
  }
}
