using System;
using System.Collections.Generic;
using System.IO;
using Trinity;
using Trinity.Network;
using System.Threading;
using System.Threading.Tasks;

namespace MultiLayerServer
{
    class DataLoader
    {
        
        /// <summary>
        /// Loads the graph data from the specified input file.
        /// Every server only loads part of the file.
        /// </summary>
        /// <param name="filePath"></param>
        public void LoadFile (string filePath, GraphType graphType) {

            FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            long fileLength = file.Length;

            // Skip to the part of the file the server is supposed to load.
            long startByte = CalculateStartByte(fileLength);
            long endByte = CalculateEndByte(fileLength);
            file.Seek(startByte, SeekOrigin.Begin);

            using (var reader = new StreamReader(file)) {
                // Skip the first line as we might jumped into the middle of a line.
                reader.ReadLine();
                string line;

                long edgeCounter = 0;
                long nodeCounter = 0;
                bool firstNode = true;

                long currentNode = -1;
                int currentLayer = -1;

                List<Task> tasks = new List<Task>();
                List<string> lines = new List<string>();

                while((line = reader.ReadLine()) != null) {
                    string[] fields = line.Split();
                    long startId = long.Parse(fields[0]);
                    int startLayer = int.Parse(fields[1]);

                    if (startId != currentNode || startLayer != currentLayer) {
                        // Don't save the first node we read as we might have jumped in the middle of the edge data.
                        if (firstNode) {
                            firstNode = false;
                        } else {
                            // We need to copy the values so they don't get changed before LoadLines is called.
                            List<string> linesCopy = new List<string>(lines);
                            long nodeCopy = currentNode;
                            int layerCopy = currentLayer;

                            Task task = Task.Run(() => LoadLines(nodeCopy, layerCopy, linesCopy));
                            tasks.Add(task);
                            lines.Clear();
                            nodeCounter++;
                        }

                        // If we read beyond our end position stop after saving the node
                        if (reader.BaseStream.Position > endByte) {
                            break;
                        }
                    }
                    lines.Add(line);
                    currentNode = startId;
                    currentLayer = startLayer;
                    edgeCounter++;
                }

                Task.WaitAll(tasks.ToArray());

                Console.WriteLine("Loaded {0} Edges", edgeCounter);
                Console.WriteLine("Loaded {0} Nodes", nodeCounter);
            }
        }


        private readonly object graphLock = new object();

        /// <summary>
        /// Loads a collection of lines that represent edges for a specified node.
        /// </summary>
        /// <param name="id">The id of the node the edges belong to.</param>
        /// <param name="layer"The layer of the node.</param>
        /// <param name="lines">A List of lines that represent the edges of the node.</param>
        private void LoadLines(long id, int layer, List<string> lines) {
            Console.WriteLine("Saving Node {0} in Layer {1}", id, layer);
            List<Edge> edges = new List<Edge>();
            foreach (string line in lines) {
                Edge newEdge = LoadDirectedWeightedEdge(line);
                edges.Add(newEdge);
            }
            Node newNode = new Node(Util.GetCellId(id, layer), layer, edges);
            Graph.SaveNode(newNode);
        }
        

        /// <summary>
        /// Loads a Directed and Weighted Edge based of a line of text.
        /// </summary>
        /// <param name="line">The line that represents the edge. The edge data is seperated by spaces.</param>
        /// <returns>The created Edge.</returns>
        private Edge LoadDirectedWeightedEdge(string line) {
            string[] fields = line.Split();

            long startId = long.Parse(fields[0]);
            int startLayer = int.Parse(fields[1]);
            long endId = long.Parse(fields[2]);
            int endLayer = int.Parse(fields[3]);
            float weight = float.Parse(fields[4]);
          
            return new Edge(startId, startLayer, endId, endLayer, weight);
        }

        /// <summary>
        /// Calculates the position from where a server is responsible for loading an input file.
        /// The lenght of the input file is distributed evenly among all the servers in the cluster.
        /// </summary>
        /// <param name="fileLength">The lenght of the input file in bytes.</param>
        /// <returns></returns>
        private long CalculateStartByte(long fileLength) {
            int myServerPosition = Global.MyPartitionId;
            int serverCount = TrinityConfig.Servers.Count;
            long startByte = (fileLength / serverCount) * myServerPosition;
            return startByte;
        }

        /// <summary>
        /// Calculates the position where a server stops being responsible for loading an input file.
        /// The lenght of the input file is distributed evenly among all the servers in the cluster.
        /// </summary>
        /// <param name="fileLength">The lenght of the input file in bytes.</param>
        /// <returns></returns>
        private long CalculateEndByte(long fileLength) {
            int myServerPosition = Global.MyPartitionId;
            int serverCount = TrinityConfig.Servers.Count;
            long startByte = (fileLength / serverCount) * (myServerPosition + 1);
            return startByte;
        }

    }
}
