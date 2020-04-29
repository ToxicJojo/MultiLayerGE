using System;
using Trinity;

namespace MultiLayerClient {
    class Program {
        static void Main(string[] args) {
            // Trinity doesn't load the config file correctly if we don't tell it to.
            TrinityConfig.LoadConfig();
            TrinityConfig.CurrentRunningMode = RunningMode.Client;

            Client client = new Client();

            // Check if we run in interactive or batch mode and start the client in the selected mode.
            if (args[0] == "interactive") {
                client.RunInteractive();
            } else if(args[0] == "batch") {
                if (args.Length != 2) {
                    Console.WriteLine("[Client] Missing path to batch file.");
                    return;
                }
                string fileName = args[1];
                client.RunBatch(fileName);
            }
        }
    }
}
