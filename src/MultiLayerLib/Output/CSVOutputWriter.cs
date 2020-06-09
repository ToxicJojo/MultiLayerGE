using System.Collections.Generic;
using System.IO;

namespace MultiLayerLib.Output {
  class CSVOutputWriter: IOutputWriter {

    public void WriteOutput(AlgorithmResult algorithmResult) {
      StreamWriter writer = new StreamWriter("results/" + algorithmResult.Name + ".csv");

      foreach(List<string> resultRow in algorithmResult.ResultTable) {
        for(int i = 0; i < resultRow.Count; i++) {
          if (i < resultRow.Count - 1) {
            writer.Write("{0},", resultRow[i]);
          } else {
            writer.Write(resultRow[i]);
          }
        }
        writer.WriteLine();
      }

      writer.Flush();
      writer.Close();
    }

  }
}
