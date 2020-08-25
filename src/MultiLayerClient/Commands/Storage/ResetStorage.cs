using MultiLayerLib;

namespace MultiLayerClient.Commands {

  class ResetStorage: Command {

    public ResetStorage (): base(null)  {
      Name = "Reset Storage";
      Keyword = "resetStorage";
      Description = "Resets the graph engine storage.";
      Arguments = new string[0];
      ArgumentsDescription = new string[0];
    }

    public override void ApplyArguments(string[] arguments) {}

    public override void Run () {
      Graph.ResetGEStorage();
    }
  }
}
