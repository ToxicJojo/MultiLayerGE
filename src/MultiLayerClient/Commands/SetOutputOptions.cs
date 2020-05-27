using MultiLayerLib;

namespace MultiLayerClient.Commands {

  class SetOutputOptions: Command {

    private OutputType OutputType  { get; set; }


    public SetOutputOptions (Client client): base (client) {
      Name = "Set output options";
      Keyword = "outputOptions";
      Description = "Sets the options for the output of executed algorithms.";
      Arguments = new string[] { "string", "bool"};
      ArgumentsDescription = new string[] { "Outputtype", "TranslateLayerIds" };
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
    }

    public override void Run() {
      Client.OutputOptions = new OutputOptions(OutputType);
    }
  }
}
