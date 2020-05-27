using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using MultiLayerClient.Commands;
using Trinity;
using Trinity.Storage;
using MultiLayerLib;

namespace MultiLayerClient {
  class Client {

    public Dictionary<String, ICommand> Commands { get; set; }

    public RemoteStorage Proxy { get; set; }

    public AlgorithmOptions AlgorithmOptions { get; set; }

    public OutputOptions OutputOptions { get; set; }

    public Client () {
      AlgorithmOptions = new AlgorithmOptions(false);
      OutputOptions = new OutputOptions(OutputType.Console, true);

      Commands = new Dictionary<string, ICommand>();

      Proxy = Global.CloudStorage.ProxyList[0];

      AddCommand(new Interactive(this));
      AddCommand(new Batch(this));
      AddCommand(new Help(this));

      AddCommand(new SetOutputOptions(this));
      AddCommand(new SetAlgorithmOptions(this));

      AddCommand(new LoadStorage());
      AddCommand(new SaveStorage());
      AddCommand(new LoadGraph(this));

      AddCommand(new ShowNode());
      AddCommand(new NodeCount(this));
      AddCommand(new EdgeCount(this));
      AddCommand(new Degree(this));
      AddCommand(new EgoNetwork(this));
      AddCommand(new PageRank(this));
      AddCommand(new PageRankTopNodes(this));
      AddCommand(new HITS(this));
      AddCommand(new HITSTopAuthorities(this));
      AddCommand(new HITSTopHubs(this));
    }

    private void AddCommand(ICommand command) {
      Commands.Add(command.Keyword, command);
    }

    private void ProcessInput (string input) {
      string commandKeyword = input.Split()[0];
      string[] arguments = input.Split().Skip(1).ToArray();

      if (!Commands.ContainsKey(commandKeyword)) {
        Console.WriteLine("[Client] Unknown command: {0}", commandKeyword);
        Console.WriteLine("[Client] Type 'help commands' for a list of all avaiable commands.");
      } else {
        ICommand command = Commands[commandKeyword];
        ExecuteCommand(command, arguments);
      }
    }

    private void ExecuteCommand(ICommand command, string[] arguments) {
      if (command.VerifyArguments(arguments)) {
        command.ApplyArguments(arguments);
        if (command.Keyword != "help" && command.Keyword != "interactive" && command.Keyword != "batch") {
          Console.WriteLine("[Client] Started {0}.", command.Name);
          command.Run();
          Console.WriteLine("[Client] Finished {0}.", command.Name);
        } else {
          command.Run();
        }
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
      try {
        StreamReader reader = new StreamReader(batchFile);

        while(!reader.EndOfStream) {
          string line = reader.ReadLine();

          ProcessInput(line);
        }
      } catch (FileNotFoundException e) {
        Console.WriteLine("[Client] {0}", e.Message);
      }
      Console.WriteLine("[Client] Ending batch mode.");
    }

  }
}
