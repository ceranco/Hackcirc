using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicNetwork
{
    class NeighbourNode
    {
        public string ID { get; }
        public List<string> neighbours = new List<string>();
    }
}
