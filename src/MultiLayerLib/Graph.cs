using Trinity;
using Trinity.TSL.Lib;
using Trinity.Core.Lib;
using System.Collections.Generic;

namespace MultiLayerLib {

  public class Graph {


    public static int LayerCount {
        get {
            return Layers.Count;
        }
    }

    public static List<Layer> Layers {
        get;
        set;
    }

    public static void Init() {
      Layers = new List<Layer>();
    }

    public static void SaveNode(Node node) {
        Global.CloudStorage.SaveNode(node);
    }

    public static void AddEdges(long cellId, List<Edge> edges) {
        Node node = Graph.LoadNode(cellId);
        node.Edges.AddRange(edges);
        Graph.SaveNode(node);
    }

    public static void AddEdges(long nodeId, int nodeLayer, List<Edge> edges) {
        long cellId = Graph.GetCellId(nodeId, nodeLayer);
        Graph.AddEdges(cellId, edges);
    }

    public static long GetCellId(long nodeId, int nodeLayer) {
      string nodeName = "n" + nodeId + "l" + nodeLayer;
      return HashHelper.HashString2Int64(nodeName);
    }

    public static Node LoadNode(long cellId) {
        return Global.CloudStorage.LoadNode(cellId);
    }

    public static Node LoadNode(long nodeId, int nodeLayer) {
      long cellId = Graph.GetCellId(nodeId, nodeLayer);
      return Graph.LoadNode(cellId);
    }

    public static bool HasNode(long cellId) {
      return Global.CloudStorage.Contains(cellId);
    }

    public static bool HasNode(long nodeId, int nodeLayer) {
      long cellId = Graph.GetCellId(nodeId, nodeLayer);
      return Graph.HasNode(cellId);
    }

    public static void SaveToGEStorage () {
      Global.CloudStorage.SaveStorage();
    }

    public static void LoadFromGEStorage () {
      Global.CloudStorage.LoadStorage();
    }

    public static Node_Accessor_local_selector NodeAccessor() {
      return Global.LocalStorage.Node_Accessor_Selector();
    }

    public static Node_Accessor UseNode(long cellId, CellAccessOptions accessOptions) {
      return Global.LocalStorage.UseNode(cellId, accessOptions);
    }

    public static bool IsLocalNode(long cellId) {
      return Global.CloudStorage.IsLocalCell(cellId);
    }

    public static int GetNodePartition(long cellId) {
      return Global.CloudStorage.GetPartitionIdByCellId(cellId);
    }

  }

}
