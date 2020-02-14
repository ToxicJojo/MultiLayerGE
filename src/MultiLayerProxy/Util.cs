using System.Collections.Generic;

namespace MultiLayerProxy {

  class Util {

    public static List<List<long>> ToLongList (List<List<string>> stringLists) {
      List<List<long>> longLists = new List<List<long>>();

      foreach(List<string> stringList in stringLists) {
        List<long> longList = new List<long>();

        foreach(string str in stringList) {
          longList.Add(long.Parse(str));
        }

        longLists.Add(longList);
      }

      return longLists;
    }


    public static List<List<double>> ToDoubleList (List<List<string>> stringLists) {
      List<List<double>> doubleLists = new List<List<double>>();

      foreach(List<string> stringList in stringLists) {
        List<double> doubleList = new List<double>();

        foreach(string str in stringList) {
          doubleList.Add(double.Parse(str));
        }

        doubleLists.Add(doubleList);
      }

      return doubleLists;
    }
  }

}
