using System;
using Trinity.Storage;

namespace MultiLayerClient.Commands {

  abstract class Command: ICommand {

    public String Name { get; set; }

    public String Keyword { get; set; }
    public String Description { get; set; }

    public String[] Arguments { get; set; }
    public String[] ArgumentsDescription { get; set; }

    protected RemoteStorage Proxy { get; set; }

    public Command(RemoteStorage proxy) {
      Proxy = proxy;
    }

    public bool VerifyArguments(string[] arguments) {
      if (Arguments.Length != arguments.Length) {
        Console.WriteLine("Wrong number of arguments for {0}, expected {1} but found {2}", Name, Arguments.Length, arguments.Length);
        Console.WriteLine("Type 'help {0}' for more information about the needed arguments", Keyword);
        return false;
      }

      for(int i = 0; i < Arguments.Length; i++) {
        try {
          switch (Arguments[i]) {
            case "string":
              break;
            case "long":
              long.Parse(arguments[i]);
              break;
            case "int":
              int.Parse(arguments[i]);
              break;
            case "double":
              double.Parse(arguments[i]);
              break;
            case "float":
              float.Parse(arguments[i]);
              break;
            case "bool":
              bool.Parse(arguments[i]);
              break;
          }
        } catch (Exception e) {
          Console.WriteLine("Argument {0} is not a {1}", i, Arguments[i]);
          Console.WriteLine("Type 'help {0}' for more information about the needed arguments", Keyword);
          return false;
        }
      }

      return true;
    }

    public abstract void ApplyArguments(string[] arguments);
    public abstract void Run ();
  }

}
