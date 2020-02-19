using System;
using System.Collections.Generic;
using System.IO;

namespace MultiLayerProxy.Output {

  class ConsoleOutputWriter: OutputWriter {

    public ConsoleOutputWriter (AlgorithmResult result): base(result) {
      this.Writer = new StreamWriter(Console.OpenStandardOutput());
      this.Writer.AutoFlush = true;
      Console.SetOut(this.Writer);
    }

    public override void WriteOutput() {
      Writer.WriteLine("Algorithm Output for: {0}", Result.Name);

      foreach(List<string> resultRow in Result.Results) {
        foreach(string resultCol in resultRow) {
          Writer.Write(resultCol + " ");
        }
        Writer.WriteLine();
      }
    } 
  }
}
