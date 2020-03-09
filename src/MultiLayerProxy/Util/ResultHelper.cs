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

  }
}
