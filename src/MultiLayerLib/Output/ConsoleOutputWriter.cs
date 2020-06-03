using System;
using System.Collections.Generic;

namespace MultiLayerLib.Output {
  class ConsoleOutputWriter: IOutputWriter {

    public void WriteOutput(AlgorithmResult algorithmResult) {

      foreach(List<string> resultRow in algorithmResult.ResultTable) {
        foreach(string resultCol in resultRow) {
          Console.Write("{0} ", resultCol);
        }
        Console.WriteLine();
      }
    }
  }
}
