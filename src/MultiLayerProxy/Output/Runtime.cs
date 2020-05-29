using System;

namespace MultiLayerProxy {

  class Runtime {

    public Runtime(DateTime startTime, DateTime endTime) {
      this.StartTime = startTime;
      this.EndTime = endTime;
      this.TimeSpan = this.EndTime - this.StartTime;
    }

    public TimeSpan TimeSpan { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get ; set; }
  }

}
