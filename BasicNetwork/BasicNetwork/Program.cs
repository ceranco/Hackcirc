
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicNetwork
{
    class Program
    {
        static void Main(string[] args)
        {
            NetworkGraph graph = new NetworkGraph(5);
            graph._routingTable[0][1] = true;
            graph._routingTable[1][0] = true;
            graph._routingTable[1][4] = true;
            graph._routingTable[4][1] = true;
            graph._routingTable[1][2] = true;
            graph._routingTable[2][1] = true;
            graph._routingTable[2][4] = true;
            graph._routingTable[4][2] = true;
            graph._routingTable[2][3] = true;
            graph._routingTable[3][2] = true;
            graph._routingTable[3][4] = true;
            graph._routingTable[4][3] = true;


            var nextNode = graph.GetNextNode(3, 0);
            nextNode = graph.GetNextNode(2, 0);
            nextNode = graph.GetNextNode(1, 0);

          }
    }
}
