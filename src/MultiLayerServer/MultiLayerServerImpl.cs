using System;
using Trinity;

namespace MultiLayerServer {

  public class MultiLayerServerImpl: MultiGraphServerBase {


    public override void LoadGraphServerHandler() {
        Console.WriteLine("Starting to load data");
        DataLoader loader = new DataLoader();
        loader.LoadFile("/home/thiel/MultiLayerGE/data/multiplex6.edges", GraphType.DirectedWeighted);
    }
  }
}
