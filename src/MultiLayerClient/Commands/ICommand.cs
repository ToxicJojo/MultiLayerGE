using System;
using System.Collections;

namespace MultiLayerClient.Commands {

  interface ICommand {

    String Name { get; set; }

    String Keyword { get; set; }

    bool VerifyArguments(string[] arguments);

    void ApplyArguments(string[] arguments);

    void Run ();

  }
}
