using System;
using System.Collections.Generic;

namespace MultiLayerProxy.Algorithms {

  interface IAlgorithm {
      
    AlgorithmType AlgorithmType { get; }

    void Run();

    TimeSpan TimedRun();

    List<double> Result { get; }

  }
}
