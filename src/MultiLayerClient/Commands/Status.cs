using System;
using Trinity;
using MultiLayerLib;

namespace MultiLayerClient.Commands {
  class Status: Command {


      public Status (): base (null) {
        Name = "Status";
        Keyword = "status";
        Description = "Shows information about the status of the ge cluster";
        Arguments = new string[0];
        ArgumentsDescription = new string[0];
      }

    public override void ApplyArguments(string[] arguments) {
    }

    public override void Run() {
      int serverCount = Global.ServerCount;
      int proxyCount = Global.ProxyCount;

      Console.WriteLine("------");
      Console.WriteLine("Proxy Count: {0}", proxyCount);
      Console.WriteLine("Server Count: {0}", serverCount);
      Console.WriteLine("------");
    }
  }

}
