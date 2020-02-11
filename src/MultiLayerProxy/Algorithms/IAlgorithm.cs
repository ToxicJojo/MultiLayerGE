using System;
using System.Collections.Generic;
using MultiLayerProxy.Output;

namespace MultiLayerProxy.Algorithms {

  interface IAlgorithm {
      
    AlgorithmType AlgorithmType { get; }

    void Run();

    TimeSpan TimedRun();

    AlgorithmResult Result { get; }

  }
}
