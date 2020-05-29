using System;
using System.Collections.Generic;
using System.IO;
using MultiLayerProxy.Algorithms;
using MultiLayerLib;

namespace MultiLayerProxy.Output {

  class ConsoleOutputWriter: OutputWriter {

    private TextWriter oldOut;

    public ConsoleOutputWriter (IAlgorithm result): base(result) {
      this.Writer = new StreamWriter(Console.OpenStandardOutput());
      this.Writer.AutoFlush = true;
      this.oldOut = Console.Out;
      Console.SetOut(this.Writer);
    }

    public override void WriteOutput(OutputOptions options) {
      List<List<string>> result = this.Algorithm.GetResult(options);
      Writer.WriteLine("Algorithm Output for: {0}", Result.Name);

      foreach(List<string> resultRow in result) {
        foreach(string resultCol in resultRow) {
          Writer.Write(resultCol + " ");
        }
        Writer.WriteLine();
      }

      Writer.Close();
      Console.SetOut(oldOut);
    } 
  }
}
