namespace MultiLayerClient.Commands {

  class Batch: Command {

    private string BatchFile { get; set; }

    public Batch (Client client): base(client)  {
      Name = "Batch Mode";
      Keyword = "batch";
      Description = "Starts a batch session with a given input file.";
      Arguments = new string[1] { "string" };
      ArgumentsDescription = new string[1] { "BatchFile" };
    }

    public override void ApplyArguments(string[] arguments) {
      BatchFile = arguments[0];
    }

    public override void Run () {
      Client.RunBatch(BatchFile);
    }
  }
}
