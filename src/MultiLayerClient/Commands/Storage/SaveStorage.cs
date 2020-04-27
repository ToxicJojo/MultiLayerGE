using Trinity;

namespace MultiLayerClient.Commands {

  class SaveStorage: Command {

    private Client Client { get; set; }

    public SaveStorage (): base(null)  {
      Name = "Save Storage";
      Keyword = "saveStorage";
      Description = "Saves the graph engine storage.";
      Arguments = new string[0];
      ArgumentsDescription = new string[0];
    }

    public override void ApplyArguments(string[] arguments) {}

    public override void Run () {
      Global.CloudStorage.SaveStorage();
    }
  }
}
