﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace BasicNetwork
{
    class Node
    {
        readonly UdpClient _client = new UdpClient(2000);
        readonly List<NeighbourNode> _neighbours = new List<NeighbourNode>();
        //NetworkGraph _network = new NetworkGraph();

        public int ID { get; }

    }
}
