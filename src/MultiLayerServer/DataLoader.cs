using System;
using System.Collections.Generic;
using System.IO;
using Trinity;
using Trinity.Network;

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

                bool firstNode = true;

                long currentNode = -1;
                int currentLayer = -1;
                List<Edge> edges = new List<Edge>();
                while((line = reader.ReadLine()) != null) {
                    Edge newEdge;
                    // Figure out how many bites have been read an stop the loop when we reach the end of the part this
                    // server is responsibvle for.

                    switch (graphType) {
                        case GraphType.DirectedWeighted:
                            newEdge = LoadDirectedWeightedEdge(line);
                            break;
                        default:
                            newEdge = new Edge(0, 0, 0, 0, 0);
                            break;
                    }

                    // If we start to load the edges of a new node save the previous one. 
                    if (currentNode != newEdge.StartId && currentNode != -1) {
                        // Don't save the first node we read as we might have jumped in the middle of the edge data.
                        if (!firstNode) {
                            // If the edges already exists we only need to add the new edges.
                            if (Graph.HasNode(currentNode, currentLayer)) {
                                Graph.AddEdges(currentNode, currentLayer, edges);
                            } else {
                                Node newNode = new Node(Util.GetCellId(currentNode, currentLayer), currentLayer, edges);
                                Graph.SaveNode(newNode);
                            }

                        } else {
                            firstNode = false;
                        }

                        edges.Clear();
                        
                        // If we read beyond our end position stop after saving the node
                        if (reader.BaseStream.Position > endByte) {
                            Console.WriteLine("Last Read Line: {0}", line);
                            break;
                        }
                    }

                    edges.Add(newEdge);
                    currentNode = newEdge.StartId;
                    currentLayer = newEdge.StartLayer;
                    edgeCounter++;
                }
                Console.WriteLine("Loaded {0} Edges", edgeCounter);
            }
        }


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
