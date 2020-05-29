using System.IO;
using System.Collections.Generic;
using MultiLayerProxy.Algorithms;
using MultiLayerLib;

namespace MultiLayerProxy.Output {

  class CSVOutputWriter: OutputWriter {

    public CSVOutputWriter(IAlgorithm result): base(result) {
      this.Writer = new StreamWriter("results/" + Algorithm.Name + ".csv");
    }

    public override void WriteOutput(OutputOptions options) {
      List<List<string>> result = this.Algorithm.GetResult(options);
      foreach(List<string> resultRow in result) {
        for (int i = 0; i < resultRow.Count; i++) {
          if (i < resultRow.Count - 1) {
            Writer.Write(resultRow[i] + ",");
          } else {
            Writer.Write(resultRow[i]);
          }
        }
        Writer.WriteLine();
      }

      Writer.Flush();
      Writer.Close();
      Writer.Dispose();
    }
  }
}
