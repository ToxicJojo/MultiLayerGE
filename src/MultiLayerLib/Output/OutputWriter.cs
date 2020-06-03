namespace MultiLayerLib.Output {
  public class OutputWriter {

    public static void WriteOutput(AlgorithmResult algorithmResult, OutputOptions options) {
      IOutputWriter outputWriter;

      switch (options.OutputType) {
        case OutputType.Console:
          outputWriter = new ConsoleOutputWriter();
          break;
        case OutputType.CSV:
          outputWriter = new CSVOutputWriter();
          break;
        default:
          outputWriter = new NoneOutputWriter();
          break;
      }

      outputWriter.WriteOutput(algorithmResult);
    }

  }
}
