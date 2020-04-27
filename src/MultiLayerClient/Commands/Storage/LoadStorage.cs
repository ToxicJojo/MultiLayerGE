using Trinity;

namespace MultiLayerClient.Commands {

  class LoadStorage: Command {

    private Client Client { get; set; }

    public LoadStorage (): base(null)  {
      Name = "Load Storage";
      Keyword = "loadStorage";
      Description = "Loads the graph engine storage.";
      Arguments = new string[0];
      ArgumentsDescription = new string[0];
    }

    public override void ApplyArguments(string[] arguments) {}

    public override void Run () {
      Global.CloudStorage.LoadStorage();
    }
  }
}
