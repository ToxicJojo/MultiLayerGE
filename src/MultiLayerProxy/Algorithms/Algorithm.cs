using System;
using System.Collections.Generic;
using MultiLayerProxy.Proxy;
using MultiLayerLib;

namespace MultiLayerProxy.Algorithms {
  abstract class Algorithm: IAlgorithm {

    protected MultiLayerProxyImpl Proxy;

    public String Name { get; protected set; }

    public Runtime Runtime { get; private set; }

    public Algorithm (MultiLayerProxyImpl proxy) {
      this.Proxy = proxy;
    }

    public virtual List<List<string>> GetResult(OutputOptions options) {
      return null;
    }

    public abstract void Run();

    public void TimedRun() {
      DateTime startTime = DateTime.Now;

      Run();

      DateTime endTime = DateTime.Now;
      this.Runtime = new Runtime(startTime, endTime);
    }
  }
}
