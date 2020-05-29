using System.Collections.Generic;
using Trinity;
using MultiLayerProxy.Proxy;
using MultiLayerProxy.Output;
using MultiLayerLib;
using MultiLayerLib.MultiLayerServer;
using MultiLayerProxy.Util;

namespace MultiLayerProxy.Algorithms {
  /// <summary>
  /// Counts all the edges in the graph. The edges are counted for each layer seperatly.
  /// </summary>
  class EdgeCount: Algorithm {

    private long[] edgeCount;

    public EdgeCount (MultiLayerProxyImpl proxy): base(proxy) {
      this.Name = "EdgeCount";
    }

    public override void Run() {
      foreach(var server in Global.CloudStorage) {
        MessagePassingExtension.GetEdgeCountServer(server);
      }

      List<List<long>> phaseResults =  Proxy.WaitForPhaseResultsAsLong(Phases.EdgeCount);
      long[] edgeCount = new long[phaseResults[0].Count];

      // Sum up the results from all servers.
      foreach(List<long> result in phaseResults) {
        for (int i = 0; i < result.Count; i++) {
            edgeCount[i] += result[i];
        }
      }
    }

    public override List<List<string>> GetResult(OutputOptions options) {
      List<List<string>> output = new List<List<string>>();
      long totalEdgeCount = 0;

      for (int i = 0; i < edgeCount.Length; i++) {
          List<string> outputRow = new List<string>();
          outputRow.Add("Layer " + (i + 1).ToString());
          outputRow.Add(edgeCount[i].ToString());
          output.Add(outputRow);
          totalEdgeCount += edgeCount[i];
      }

      List<string> totalOutputRow = ResultHelper.Row("Total", totalEdgeCount.ToString());
      output.Add(totalOutputRow);

      return output;
    }
  }
}
