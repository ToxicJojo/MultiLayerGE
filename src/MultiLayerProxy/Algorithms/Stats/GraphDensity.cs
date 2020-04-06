using System;
using System.Collections.Generic;
using Trinity;
using MultiLayerProxy.Proxy;
using MultiLayerProxy.Output;

namespace MultiLayerProxy.Algorithms {

  class GraphDensity: Algorithm {

    public GraphDensity (MultiLayerProxyImpl proxy): base(proxy) {
    }

    public override void Run() {
      foreach(var server in Global.CloudStorage) {
        MultiLayerServer.MessagePassingExtension.GetNodeCountServer(server);
      }

      List<List<long>> nodePhaseResults =  Proxy.WaitForPhaseResultsAsLong(Phases.NodeCount);
      long[] nodeCount = new long[nodePhaseResults[0].Count];

      // Sum up the results from all the servers.
      foreach(List<long> result in nodePhaseResults) {
        for (int i = 0; i < result.Count; i++) {
            nodeCount[i] += result[i];
        }
      }

      foreach(var server in Global.CloudStorage) {
        MultiLayerServer.MessagePassingExtension.GetEdgeCountServer(server);
      }

      List<List<long>> edgePhaseResults =  Proxy.WaitForPhaseResultsAsLong(Phases.EdgeCount);
      long[] edgeCount = new long[edgePhaseResults[0].Count];

      // Sum up the results from all servers.
      foreach(List<long> result in edgePhaseResults) {
        for (int i = 0; i < result.Count; i++) {
            edgeCount[i] += result[i];
        }
      }
      
      double[] density = new double[nodeCount.Length];
      for (int i = 0; i < density.Length; i++) {
          density[i] =  Convert.ToDouble(edgeCount[i]) / (Convert.ToDouble(nodeCount[i]) * (Convert.ToDouble(nodeCount[i]) - 1));
      }

      WriteOutput(density);
    }

    private void WriteOutput(double[] edgeDegree) {
      List<List<string>> output = new List<List<string>>();

      for (int i = 0; i < edgeDegree.Length; i++) {
          List<string> outputRow = new List<string>();
          outputRow.Add("Layer " + (i + 1).ToString());
          outputRow.Add(edgeDegree[i].ToString());
          output.Add(outputRow);
      }

      Result = new AlgorithmResult("GraphDensity", output);
    }
  }
}
