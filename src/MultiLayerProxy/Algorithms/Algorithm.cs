using System;
using System.Diagnostics;
using System.Collections.Generic;
using MultiLayerProxy.Proxy;
using MultiLayerProxy.Output;

namespace MultiLayerProxy.Algorithms {
  abstract class Algorithm: IAlgorithm {

    protected MultiLayerProxyImpl Proxy;

    public AlgorithmResult Result { get; protected set; }

    public Algorithm (MultiLayerProxyImpl proxy) {
      this.Proxy = proxy;
    }

    public abstract void Run();

    public TimeSpan TimedRun() {
      Stopwatch stopwatch = new Stopwatch();
      stopwatch.Start();

      Run();

      stopwatch.Stop();
      return stopwatch.Elapsed;
    }
  }
}
