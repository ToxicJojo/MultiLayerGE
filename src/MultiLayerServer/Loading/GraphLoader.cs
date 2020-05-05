using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using Trinity;
using MultiLayerLib;


namespace MultiLayerServer.Loading {

  class GraphLoader {

    private IEdgeLoader edgeLoader;

    public GraphLoader(IEdgeLoader edgeLoader) {
      this.edgeLoader = edgeLoader;
    }

    public void LoadGraph(string configFilePath) {
      GraphConfig graphConfig = GraphConfig.LoadConfig(configFilePath);
      //Graph.Init();

      LoadLayers(graphConfig.LayersFilePath);
      LoadEdges(graphConfig.EdgesFilePath);
    }

    /// <summary>
    /// Loads the layer information for the graph from a file.
    /// </summary>
    /// <param name="layersFilePath">The full path to the layer info file.</param>
    private void LoadLayers(string layersFilePath) {
      Console.WriteLine("[GraphLoader] Loading Layers");
      StreamReader reader = new StreamReader(layersFilePath);
      // Skip the description line
      reader.ReadLine();

      while (!reader.EndOfStream) {
        string line = reader.ReadLine();
        string[] fields = line.Split();

        // Each layer has an id and a label we need to load.
        int layerId = int.Parse(fields[0]);
        string layerLabel = fields[1];

        Layer layer = new Layer(layerId, layerLabel);
        Graph.Layers.Add(layer);
      }

      Console.WriteLine("[GraphLoader] Loaded {0} Layers", Graph.LayerCount);
    }


    private bool fileDone = false;
    private long nodesCompleted = 0;

    private void LoadEdges(string edgeFilePath) {
      Console.WriteLine("[GraphLoader] Loading edges.");
      FileStream file = new FileStream(edgeFilePath, FileMode.Open, FileAccess.Read);
      long fileLength = file.Length;


      using (StreamReader reader = new StreamReader(file)) {
        // We need to keep track of all the tasks that load the nodes.
        List<Task> loadingTasks = new List<Task>();
        List<string> lines = new List<string>();

        long currentNode = -1;
        int currentLayer = -1;
        bool firstNode = true;

        long nodeCount = 0;


        // We loop over the lines of the edge file and collect bundels of lines that represent one node.
        // Those bundles will be send over to a 
        while(!reader.EndOfStream) {
          string line = reader.ReadLine();

          long nodeId = edgeLoader.GetId(line);
          int layerId = edgeLoader.GetLayer(line);
          long cellId = Graph.GetCellId(nodeId, layerId);

          if (!Global.CloudStorage.IsLocalCell(cellId)) {
            continue;
          }

          if (nodeId != currentNode || layerId != currentLayer) {
            // Don't save the first node we read as we might have jumped in the middle of the edge data.
            if (firstNode) {
              firstNode = false;
            } else {
              // Create copies of the values to load the lines as we might overwrite them before LoadLines is called on the other thread.
              List<string> linesCopy = new List<string>(lines);
              long nodeCopy = currentNode;
              int layerCopy = currentLayer;

              Task task = Task.Run(() => LoadLines(nodeCopy, layerCopy, linesCopy));
              loadingTasks.Add(task);
              nodeCount++;

              lines.Clear();
            }
          }

          currentNode = nodeId;
          currentLayer = layerId;
          lines.Add(line);
        }

        Console.WriteLine("[GraphLoader] Waiting for tasks to finish");
        // Wait until all threads are done loading nodes.
        Task[] taskArray = loadingTasks.ToArray();
        Task.WaitAll(taskArray);
        Console.WriteLine("[GraphLoader] Loaded {0} nodes.", nodeCount);
      }
    }


    /// <summary>
    /// Loads a collection of lines that represent edges for a specified node.
    /// </summary>
    /// <param name="id">The id of the node the edges belong to.</param>
    /// <param name="layer"The layer of the node.</param>
    /// <param name="lines">A List of lines that represent the edges of the node.</param>
    private void LoadLines(long id, int layer, List<string> lines) {
        List<Edge> edges = new List<Edge>();
        foreach (string line in lines) {
            Edge newEdge = edgeLoader.LoadEdge(line);
            edges.Add(newEdge);
        }
        if (!Graph.HasNode(id, layer)) {
            // If the node is new just add it to the Graph
            PageRankData pageRankData = new PageRankData(0, 0);
            HITSData hitsData = new HITSData(0, 0, 0, 0);
            DegreeData degreeData = new DegreeData(0, 0, 0);
            Node newNode = new Node(Graph.GetCellId(id, layer), id, layer, pageRankData, hitsData, degreeData, edges);
            Graph.SaveNode(newNode);
        } else {
            // Otherwise add the edges to the existing node.
            Graph.AddEdges(id, layer, edges);
        }
    }
  }
}
