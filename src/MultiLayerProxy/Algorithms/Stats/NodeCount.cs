using System.Collections.Generic;
using Trinity;
using MultiLayerProxy.Proxy;
using MultiLayerLib;
using MultiLayerLib.MultiLayerServer;
using MultiLayerProxy.Util;

namespace MultiLayerProxy.Algorithms {

  class NodeCount: Algorithm {

    private long[] nodeCount;

    public NodeCount (MultiLayerProxyImpl proxy): base(proxy) {
      this.Name = "NodeCount";
    }

    public override void Run() {
      foreach(var server in Global.CloudStorage) {
        MessagePassingExtension.GetNodeCountServer(server);
      }

      List<List<long>> phaseResults =  Proxy.WaitForPhaseResultsAsLong(Phases.NodeCount);
      nodeCount = new long[phaseResults[0].Count];

      // Sum up the results from all the servers.
      foreach(List<long> result in phaseResults) {
        for (int i = 0; i < result.Count; i++) {
            nodeCount[i] += result[i];
        }
      }
    }

    public override List<List<string>>  GetResultTable(OutputOptions options) {
      List<List<string>> output = new List<List<string>>();
      long totalNodeCount = 0;

      for (int i = 0; i < nodeCount.Length; i++) {
          List<string> outputRow = ResultHelper.Row("Layer" + (i + 1), nodeCount[i].ToString()); 
          output.Add(outputRow);

          totalNodeCount += nodeCount[i];
      }


      output.Add(ResultHelper.Row("Toal", totalNodeCount.ToString()));

      return output;
    }
  }
}
