using System;
using System.Collections.Generic;

namespace MultiLayerProxy.Output {
  
  class AlgorithmResult {


    public string Name { get; set; }

    public TimeSpan Runtime { get; set; }

    public List<List<string>> Results { get; set; }

    public AlgorithmResult(string name, List<List<string>> results) {
      this.Name = name;
      this.Results = results;
    }

    public AlgorithmResult(string name, TimeSpan runtime, List<List<string>> results) {
      this.Name = name;
      this.Runtime = runtime;
      this.Results = results;
    }

  }
}
