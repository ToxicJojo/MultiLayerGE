using System;
using System.Collections.Generic;
using MultiLayerProxy.Output;

namespace MultiLayerProxy.Algorithms {

  interface IAlgorithm {
      
    void Run();

    TimeSpan TimedRun();

    AlgorithmResult Result { get; }

  }
}
