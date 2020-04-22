using System;
using System.Linq;
using System.IO;
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

      AddCommand(new Interactive(this));
      AddCommand(new Batch(this));

      AddCommand(new ShowNode(Proxy));
      AddCommand(new NodeCount(Proxy));
      AddCommand(new EdgeCount(Proxy));
      AddCommand(new Degree(Proxy));
      AddCommand(new EgoNetwork(Proxy));
      AddCommand(new PageRank(Proxy));
      AddCommand(new PageRankTopNodes(Proxy));
      AddCommand(new HITS(Proxy));
      AddCommand(new HITSTopAuthorities(Proxy));
      AddCommand(new HITSTopHubs(Proxy));
    }

    private void AddCommand(ICommand command) {
      Commands.Add(command.Keyword, command);
    }

    private void ProcessInput (string input) {
      string commandKeyword = input.Split()[0];
      string[] arguments = input.Split().Skip(1).ToArray();

      if (!Commands.ContainsKey(commandKeyword)) {
        Console.WriteLine("[Client] Unknown command: {0}", commandKeyword);
      } else {
        ICommand command = Commands[commandKeyword];
        ExecuteCommand(command, arguments);
      }
    }

    private void ExecuteCommand(ICommand command, string[] arguments) {
      if (command.VerifyArguments(arguments)) {
        command.ApplyArguments(arguments);
        command.Run();
      }
    }

    public void RunInteractive() {
      Console.WriteLine("[Client] Started interactive mode.");

      string input = "";
      Console.Write("> ");
      while((input = Console.ReadLine()) != "exit") {
        
        ProcessInput(input);

        Console.Write("> ");
      }
      Console.WriteLine("[Client] Ending interactiv mode.");
    }

    public void RunBatch (string batchFile) {
      Console.WriteLine("[Client] Started batch mode.");
      StreamReader reader = new StreamReader(batchFile);

      while(!reader.EndOfStream) {
        string line = reader.ReadLine();

        ProcessInput(line);
      }

      Console.WriteLine("[Client] Ending batch mode.");
    }

  }
}
