using System;

namespace MultiLayerClient.Commands {


  class Help: Command {

    public string HelpArgument { get; set; }

    public Client Client { get; set; }

    public Help(Client client): base(null) {
      Name = "Help";
      Keyword = "help";
      Description = "";
      Arguments = new string[] { "string" };
      ArgumentsDescription = new string[] { "HelpArgument" };
      Client = client;
    }


    public override void ApplyArguments(string[] arguments) {
      HelpArgument = arguments[0];
    }

    public override void Run() {
      if (HelpArgument == "commands") {
        ListCommads();
      } else if (Client.Commands.ContainsKey(HelpArgument)) {
        ICommand command = Client.Commands[HelpArgument];
        Console.WriteLine("{0} - {1}", command.Name, command.Description);
        Console.WriteLine("Arguments: ");

        for (int i = 0; i < command.Arguments.Length; i++) {
           Console.WriteLine("  {0}. {1} [{2}]", i, command.ArgumentsDescription[i], command.Arguments[i]);
        }


      }
    }


    private void ListCommads ()  {
      Console.WriteLine("Listing all avaiable commands:");
      foreach(ICommand command in Client.Commands.Values) {
        Console.WriteLine("  {0} - {1}", command.Keyword, command.Description);
      }
      Console.WriteLine("  exit - Exits an interactive session.");
    }



  }

}
