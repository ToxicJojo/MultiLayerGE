using System;
using System.IO;

namespace MultiLayerServer {

  class GraphConfig {
    
    /// <summary>
    /// The path to the file containig the edges.
    /// </summary>
    public string EdgesFilePath { get; set; } 

    /// <summary>
    /// The path to the file containig the information about the graph layers.
    /// </summary>
    public string LayersFilePath { get; set; } 
    
    /// <summary>
    /// The path to the file containing the information about the graph nodes.
    /// </summary>
    public string LayoutFilePath { get; set; }

    private GraphConfig(string edgesFilePath, string layersFilePath, string layoutFilePath) {
      this.EdgesFilePath = edgesFilePath;
      this.LayersFilePath = layersFilePath;
      this.LayoutFilePath = layoutFilePath;
    }

    /// <summary>
    /// Loads the configuration based on the data in a config file.
    /// </summary>
    /// <param name="configFilePath">The path to the config file.</param>
    /// <returns>The loaded configuration based on the data in the config file.</returns>
    public static GraphConfig LoadConfig(string configFilePath) {
      StreamReader reader = new StreamReader(configFilePath);
      string line = reader.ReadLine();
      string[] filePaths = line.Split(';');

      return new GraphConfig(filePaths[0], filePaths[1], filePaths[2]);
    }
  }
}
