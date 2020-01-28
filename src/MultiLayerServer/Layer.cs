
namespace MultiLayerServer {


  class Layer {


    public Layer (int layerId, string layerLabel) {
      this.Id = layerId;
      this.Label = layerLabel;
    }

    public int Id { get; set; }

    public string Label { get; set; }

  }

}
