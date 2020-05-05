using MultiLayerLib;

namespace MultiLayerClient.Commands {

  class LoadStorage: Command {

    public LoadStorage (): base(null)  {
      Name = "Load Storage";
      Keyword = "loadStorage";
      Description = "Loads the graph engine storage.";
      Arguments = new string[0];
      ArgumentsDescription = new string[0];
    }

    public override void ApplyArguments(string[] arguments) {}

    public override void Run () {
      Graph.LoadFromGEStorage();
    }
  }
}
