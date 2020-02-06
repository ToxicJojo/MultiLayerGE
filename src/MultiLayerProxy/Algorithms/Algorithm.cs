using System;
using System.Diagnostics;
using System.Collections.Generic;
using MultiLayerProxy.Proxy;

namespace MultiLayerProxy.Algorithms {
  abstract class Algorithm: IAlgorithm {

    protected MultiLayerProxyImpl Proxy;

    public AlgorithmType AlgorithmType { get; protected set; }

    public List<double> Result { get; protected set; }

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
