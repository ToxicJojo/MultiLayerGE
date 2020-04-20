using System;
using System.Linq;
using System.Collections.Generic;
using MultiLayerClient.Commands;

namespace MultiLayerClient {
  class Client {

    private Dictionary<String, ICommand> Commands { get; set; }

    public Client () {
      Commands = new Dictionary<string, ICommand>();

      AddCommand(new ShowNode());
      AddCommand(new NodeCount());
      AddCommand(new EdgeCount());
      AddCommand(new PageRank());
      AddCommand(new PageRankTopNodes());
    }

    private void AddCommand(ICommand command) {
      Commands.Add(command.Keyword, command);
    }

    private void ExecuteCommand(ICommand command, string[] arguments) {
      if (command.VerifyArguments(arguments)) {
        command.ApplyArguments(arguments);
        command.Run();
      }
    }

    public void RunInteractive() {
      Console.WriteLine("[Client] Started  in interactive mode.");

      string input = "";
      while((input = Console.ReadLine()) != "exit") {
        string commandKeyword = input.Split()[0];
        string[] arguments = input.Split().Skip(1).ToArray();
        
        if (!Commands.ContainsKey(commandKeyword)) {
          Console.WriteLine("[Client] Unknown command: {0}", commandKeyword);
        } else {
          ICommand command = Commands[commandKeyword];
          ExecuteCommand(command, arguments);
        }
      }
    }

    public void RunBatch (string batchFile) {

    }

  }
}
