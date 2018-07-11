using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicNetwork
{
    class NetworkGraph
    {
        bool[][] _routingTable;
        readonly int _maxNodes;

        public NetworkGraph(int maxNodes)
        {
            _maxNodes = maxNodes;
            _routingTable = new bool[maxNodes][];
            for (int i = 0; i < maxNodes; i++)
            {
                _routingTable[i] = new bool[maxNodes];
                for (int j = 0; j < maxNodes; j++)
                {
                    _routingTable[i][j] = false;
                }
            }
        }

        public void Set(bool[][] table)
        {
            _routingTable = table;
        }

        public int GetNextNode(int src, int dst, List<int> prevNodes = null)
        {
            if (prevNodes == null)
            {
                prevNodes = new List<int>();
            }

            int nextDst;
            for (int i = 0; i < _maxNodes; i++)
            {
                if (_routingTable[dst][i] == true)
                {
                    nextDst = i;
                    
                    if (src == nextDst)
                    {
                        return dst;
                    }

                    if (!prevNodes.Contains(nextDst))
                    {
                        prevNodes.Add(dst);
                        int nextNode = GetNextNode(src, nextDst, prevNodes);
                        if (nextNode != -1)
                        {
                            return nextNode;
                        }
                    }
                }
            }

            return -1;
        }

        public bool this[int i, int j]
        {
            get { return _routingTable[i][j]; }
            set { _routingTable[i][j] = value; }
        }
    }
}
