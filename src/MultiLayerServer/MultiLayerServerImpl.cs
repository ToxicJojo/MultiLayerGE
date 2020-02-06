using System;
using System.Collections.Generic;
using Trinity;
/*
namespace MultiLayerServer {

  public partial class MultiLayerServerImplOld: MultiGraphServerBase {

    private void PhaseFinished(string phase) {
      PhaseFinished(phase, new List<double>());
    }

    private void PhaseFinished(string phase, List<double> result) {
      using (var msg = new PhaseFinishedMessageWriter(result, phase)) {
        MultiGraphProxy.MessagePassingExtension.PhaseFinished(Global.CloudStorage.ProxyList[0], msg);
      }
    }

    public override void LoadGraphServerHandler() {
        Console.WriteLine("Starting to load data");
        DataLoader loader = new DataLoader();
        loader.LoadFile("/home/thiel/MultiLayerGE/data/multiplex6.edges", GraphType.DirectedWeighted);
        
        
        PhaseFinished("phaseDataLoad");
    }


    public override void GetNodeCountHandler() {
      double[] nodeCount = new double[Graph.LayerCount];

      foreach(Node node in Global.LocalStorage.Node_Selector()) {
        nodeCount[node.Layer - 1]++;
      }

      List<double> result = new List<double>(nodeCount);
      PhaseFinished("phaseNodeCount", result);
    }

    public override void GetEdgeCountHandler() {
      double[] edgeCount = new double[Graph.LayerCount];

      foreach(Node node in Global.LocalStorage.Node_Selector()) {
        foreach(Edge edge in node.Edges) {
          // Only count edges which are between different nodes.
          // This will exclude the edges between the same nodes on different layers.
          if (edge.StartId != edge.DestinationId) {
            edgeCount[edge.StartLayer - 1]++;
          }
        }
      }

      List<double> result = new List<double>(edgeCount);
      PhaseFinished("phaseEdgeCount", result);
    }


  }
}
*/
