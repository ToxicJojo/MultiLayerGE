using System.Collections.Generic;
using MultiLayerLib;

namespace MultiLayerProxy.Algorithms {

  interface IAlgorithm {

    string Name { get; }
      
    void Run();

    void TimedRun();

    List<List<string>> GetResult(OutputOptions options);

    Runtime Runtime { get; }

  }
}
