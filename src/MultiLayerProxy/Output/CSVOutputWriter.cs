using System.IO;
using System.Collections.Generic;

namespace MultiLayerProxy.Output {

  class CSVOutputWriter: OutputWriter {

    public CSVOutputWriter(AlgorithmResult result): base(result) {
      this.Writer = new StreamWriter("results/" + result.Name + ".csv");
    }

    public override void WriteOutput() {
      foreach(List<string> resultRow in Result.Results) {
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
