using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicNetwork
{
    class NetworkGraph
    {
        private int[][] edges;
        private IDictionary<string, int> map;
        private LinkedList<int> freeCells;
        private int freeIndex;

        public NetworkGraph() { }

        public void Generate(Node root, List<NeighbourNode> neighbours)
        {
            var connections = new Dictionary<string, string>();

        }


        public virtual int Size
        {
            get
            {
                return map.Count;
            }
        }

        public string[] getEdges()
        {
            string[] router = new string[map.Count];
            int i = 0;
            foreach (string @var in map.Keys)
            {
                router[i] = @var;
                i++;
            }
            return router;
        }

        public virtual bool addEdge(string router)
        {
            int index;
            if (map.ContainsKey(router))
            {
                return false;
            }
            if (freeCells.Count == 0)
            {
                index = freeIndex;
                map[router] = index;
                freeIndex++;
            }
            else
            {
                index = freeCells.First.Value;
                freeCells.RemoveFirst();
                map[router] = index;
            }
            return true;
        }

        public virtual bool removeEdge(string router)
        {
            int index;
            if (map.ContainsKey(router))
            {
                map.TryGetValue(router, out index);
                map.Remove(router);
                freeCells.AddLast(index);
                for (int i = 0; i < edges[index].Length; i++)
                {
                    edges[index][i] = 0;
                    edges[i][index] = 0;
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public virtual bool setLink(string source, string dest, int weight)
        {
            if (map.ContainsKey(source) && map.ContainsKey(dest))
            {
                edges[map[source]][map[dest]] = weight;
                edges[map[dest]][map[source]] = weight;
                return true;
            }
            else
            {
                return false;
            }
        }

        public virtual bool removeLink(string source, string dest)
        {
            if (map.ContainsKey(source) && map.ContainsKey(dest))
            {
                edges[map[source]][map[dest]] = 0;
                edges[map[dest]][map[source]] = 0;
                return true;
            }
            else
            {
                return false;
            }
        }

        public virtual string[] getNeighbors(string router)
        {
            int[] numbers;
            string[] neighbors;
            if (map.ContainsKey(router))
            {
                int count = 0;
                int index;
                map.TryGetValue(router, out index);
                for (int i = 0; i < edges.Length; i++)
                {
                    if (edges[index][i] > 0)
                    {
                        count++;
                    }
                }
                numbers = new int[count];
                neighbors = new string[count];
                count = 0;
                for (int i = 0; i < edges.Length; i++)
                {
                    if (edges[index][i] > 0)
                    {
                        numbers[count++] = i;
                    }
                }
                count = 0;
                foreach (string id in map.Keys)
                {
                    for (int i = 0; i < numbers.Length; i++)
                    {
                        if (numbers[i] == map[id])
                        {
                            neighbors[count++] = id;
                            break;
                        }
                    }
                }
                return neighbors;
            }
            else
            {
                return null;
            }
        }
    }
}
}
