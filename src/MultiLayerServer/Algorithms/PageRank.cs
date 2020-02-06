using Trinity;

namespace MultiLayerServer.Algorithms {
  class PageRank {


    public static void SetInitialValues(double initialValue) {
      foreach(Node_Accessor node in Global.LocalStorage.Node_Accessor_Selector()) {
        node.PageRankData.Value = initialValue;
        node.PageRankData.OldValue = initialValue;
      }
    }


  }
}
