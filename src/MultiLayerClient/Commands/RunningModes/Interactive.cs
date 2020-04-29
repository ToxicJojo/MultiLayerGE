namespace MultiLayerClient.Commands {

  class Interactive: Command {

    public Interactive (Client client): base(client)  {
      Name = "Interactive Mode";
      Keyword = "interactive";
      Description = "Starts an interactive session.";
      Arguments = new string[0];
      ArgumentsDescription = new string[0];
    }

    public override void ApplyArguments(string[] arguments) {}

    public override void Run () {
      Client.RunInteractive();
    }
  }
}
