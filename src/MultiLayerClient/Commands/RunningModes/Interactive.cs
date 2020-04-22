namespace MultiLayerClient.Commands {

  class Interactive: Command {

    private Client Client { get; set; }

    public Interactive (Client client): base(null)  {
      Name = "Interactive Mode";
      Keyword = "interactive";
      Arguments = new string[0];
      Client = client;
    }

    public override void ApplyArguments(string[] arguments) {}

    public override void Run () {
      Client.RunInteractive();
    }
  }
}
