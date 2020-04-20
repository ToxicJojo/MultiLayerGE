using System;
using System.Collections.Generic;

namespace MultiLayerClient.Commands {

  abstract class Command: ICommand {

    public String Name { get; set; }

    public String Keyword { get; set; }

    public String[] Arguments { get; set; }

    public bool VerifyArguments(string[] arguments) {
      if (Arguments.Length != arguments.Length) {
        Console.WriteLine("Wrong number of arguments for {0}, expected {1} but found {2}", Name, Arguments.Length, arguments.Length);
        return false;
      }

      for(int i = 0; i < Arguments.Length; i++) {
        try {
          switch (Arguments[i]) {
            case "long":
              long.Parse(arguments[i]);
              break;
            case "int":
              int.Parse(arguments[i]);
              break;
          }
        } catch (Exception e) {
          Console.WriteLine("Argument {0} is not a {1}", i, Arguments[i]);
          return false;
        }
      }

      return true;
    }

    public abstract void ApplyArguments(string[] arguments);
    public abstract void Run ();
  }

}
