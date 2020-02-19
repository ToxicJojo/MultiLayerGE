using System;
using System.IO;


namespace MultiLayerProxy.Output {

  abstract class OutputWriter: IOutputWriter {


    protected AlgorithmResult Result { get; set; }

    protected StreamWriter Writer { get; set; }

    public OutputWriter (AlgorithmResult result) {
      this.Result = result;
    }

    public abstract void WriteOutput();

  }
}
