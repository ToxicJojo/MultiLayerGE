namespace MultiLayerClient.Commands {

  class SetAlgorithmOptions: Command {

    private bool Timed { get; set; }

    public SetAlgorithmOptions (Client client): base (client) {
      Name = "Set algorithm options";
      Keyword = "algorithmOptions";
      Description = "Sets the options for the executed algorithms.";
      Arguments = new string[] { "bool" };
      ArgumentsDescription = new string[] { "Timed" };
    }

    public override void ApplyArguments(string[] arguments) {
      Timed = bool.Parse(arguments[0]);
    }

    public override void Run() {
      Client.AlgorithmOptions = new AlgorithmOptions(Timed);
    }
  }
}
