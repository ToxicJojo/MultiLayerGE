using System;
using System.Collections.Generic;
using Trinity;
using Trinity.Core.Lib;
using Trinity.Network;

namespace MultiLayerServer
{
    class Util {

      public static long GetCellId (long id, int layer) {
        string nodeName = "n" + id + "l" + layer;
        return HashHelper.HashString2Int64(nodeName);
      }

      public static List<string> ToStringList<T>(List<T> list) {
        List<string> stringList = new List<string>();

        foreach(T obj in list) {
          stringList.Add(obj.ToString());
        }

        return stringList;
      }

    }
}
