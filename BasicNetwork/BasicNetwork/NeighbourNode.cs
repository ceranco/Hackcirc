using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicNetwork
{
    class NeighbourNode
    {
        public int ID { get; }
        public List<NeighbourNode> neighbours = new List<NeighbourNode>();
    }
}
