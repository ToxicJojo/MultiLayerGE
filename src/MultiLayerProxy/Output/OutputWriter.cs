using System.IO;
using MultiLayerProxy.Algorithms;
using MultiLayerLib;


namespace MultiLayerProxy.Output {

  abstract class OutputWriter: IOutputWriter {

    protected AlgorithmResult Result { get; set; }

    protected IAlgorithm Algorithm { get; set; }


    protected StreamWriter Writer { get; set; }

    public OutputWriter(IAlgorithm algorithm) {
      this.Algorithm = algorithm;
    }

    public abstract void WriteOutput(OutputOptions options);

  }
}
