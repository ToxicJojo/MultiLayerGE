using System;
using System.Collections.Generic;

namespace MultiLayerProxy.Util {
  class ResultHelper {

    public static double SumUpLayerResults(List<List<double>> results, int layer) {
      double sum = 0;

      foreach(List<double> result in results) {
        sum += result[layer];
      }

      return sum;
    }

    public static string FormatTimeSpan(TimeSpan timeSpan) {
      return String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
          timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds,
          timeSpan.Milliseconds / 10);
    }


    public static List<string> Row(params string[] cols) {
      List<string> row = new List<string>();
      foreach(string col in cols) {
        row.Add(col);
      }

      return row;
    }
  }
}
