using System.Collections.Generic;
using Trinity;
using MultiLayerProxy.Proxy;
using MultiLayerProxy.Output;

namespace MultiLayerProxy.Algorithms {
  /// <summary>
  /// Counts all the edges in the graph. The edges are counted for each layer seperatly.
  /// </summary>
  class EdgeCount: Algorithm {

    public EdgeCount (MultiLayerProxyImpl proxy): base(proxy) {
    }

    public override void Run() {
      foreach(var server in Global.CloudStorage) {
        MultiLayerServer.MessagePassingExtension.GetEdgeCountServer(server);
      }

      List<List<long>> phaseResults =  Proxy.WaitForPhaseResultsAsLong(Phases.EdgeCount);
      long[] edgeCount = new long[phaseResults[0].Count];

      // Sum up the results from all servers.
      foreach(List<long> result in phaseResults) {
        for (int i = 0; i < result.Count; i++) {
            edgeCount[i] += result[i];
        }
      }

      WriteOutput(edgeCount);
    }

    private void WriteOutput(long[] edgeCount) {
      List<List<string>> output = new List<List<string>>();
      long totalEdgeCount = 0;

      for (int i = 0; i < edgeCount.Length; i++) {
          List<string> outputRow = new List<string>();
          outputRow.Add("Layer " + (i + 1).ToString());
          outputRow.Add(edgeCount[i].ToString());
          output.Add(outputRow);
          totalEdgeCount += edgeCount[i];
      }

      List<string> totalOutputRow = new List<string>();
      totalOutputRow.Add("Total");
      totalOutputRow.Add(totalEdgeCount.ToString());
      output.Add(totalOutputRow);

      Result = new AlgorithmResult("EdgeCount", output);
    }
  }
}
