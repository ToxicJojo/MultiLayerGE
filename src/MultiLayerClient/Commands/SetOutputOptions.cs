using MultiLayerLib;

namespace MultiLayerClient.Commands {

  class SetOutputOptions: Command {

    private OutputType OutputType  { get; set; }

    private bool TranslateLayerIds { get; set; }

    private bool RemoteOutput { get; set; }


    public SetOutputOptions (Client client): base (client) {
      Name = "Set output options";
      Keyword = "outputOptions";
      Description = "Sets the options for the output of executed algorithms.";
      Arguments = new string[] { "string", "bool", "bool" };
      ArgumentsDescription = new string[] { "Outputtype", "TranslateLayerIds", "RemoteOutput" };
    }

    public override void ApplyArguments(string[] arguments) {
      switch(arguments[0]) {
        case "None":
          OutputType = OutputType.None;
          break;
        case "Console":
          OutputType = OutputType.Console;
          break;
        case "CSV":
          OutputType = OutputType.CSV;
          break;
      }

      TranslateLayerIds = bool.Parse(arguments[1]);
      RemoteOutput = bool.Parse(arguments[2]);
    }

    public override void Run() {
      Client.OutputOptions = new OutputOptions(OutputType, TranslateLayerIds, RemoteOutput);
    }
  }
}
