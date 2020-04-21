using System;
using System.Linq;
using System.Collections.Generic;
using MultiLayerClient.Commands;
using Trinity;
using Trinity.Storage;

namespace MultiLayerClient {
  class Client {

    private Dictionary<String, ICommand> Commands { get; set; }

    private RemoteStorage Proxy { get; set; }

    public Client () {
      Commands = new Dictionary<string, ICommand>();

      Proxy = Global.CloudStorage.ProxyList[0];

      AddCommand(new ShowNode(Proxy));
      AddCommand(new NodeCount(Proxy));
      AddCommand(new EdgeCount(Proxy));
      AddCommand(new PageRank(Proxy));
      AddCommand(new PageRankTopNodes(Proxy));
      AddCommand(new HITS(Proxy));
      AddCommand(new HITSTopAuthorities(Proxy));
      AddCommand(new HITSTopHubs(Proxy));
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
