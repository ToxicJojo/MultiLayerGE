using System.Collections.Generic;
using MultiLayerLib;

namespace MultiLayerProxy.Algorithms {
  interface IAlgorithm {

    string Name { get; }

    Runtime Runtime { get; }
      
    void Run();

    void TimedRun();

    List<List<string>> GetResultTable(OutputOptions outputOptions);

    AlgorithmResult GetResult(OutputOptions outputOptions);
  }
}
