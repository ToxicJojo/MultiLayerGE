using System;
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

    }
}
