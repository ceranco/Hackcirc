using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace BasicNetwork
{
    public class Node
    {
        #region Constants
        private static int kBroadcastPort = 2000;
        private static int kNumNodes = 5;
        #endregion

        #region Members

        // UDP clients for sending and receiving messages
        UdpClient _udpSend = new UdpClient() { EnableBroadcast = true };
        UdpClient _udpReceive = new UdpClient(kBroadcastPort) { EnableBroadcast = true };

        // IP endpoints for receiving and sending broadcasts
        IPEndPoint _receiveEndPoint = new IPEndPoint(IPAddress.Any, 0);
        Dictionary<int, IPEndPoint> _broadcast = new Dictionary<int, IPEndPoint>();

        // Threads for receiving and sending
        Thread _receiveThread = null;
        Thread _mainThread = null;
        int _id = 0;
        Int64 _idCount = 0;
        List<int> _neighbourNodes = new List<int>();

        NetworkGraph _networkGraph = new NetworkGraph(kNumNodes);
        ConcurrentQueue<Packet> _packetQueue = new ConcurrentQueue<Packet>();

        #endregion

        public Node()
        {
            _broadcast.Add(0, new IPEndPoint(IPAddress.Parse("1.1.0.29"), kBroadcastPort));
            _broadcast.Add(1, new IPEndPoint(IPAddress.Parse("1.1.0.28"), kBroadcastPort));
            _broadcast.Add(2, new IPEndPoint(IPAddress.Parse("1.1.0.35"), kBroadcastPort));
            _broadcast.Add(3, new IPEndPoint(IPAddress.Parse("1.1.0.37"), kBroadcastPort));
            _broadcast.Add(4, new IPEndPoint(IPAddress.Parse("1.1.0.64"), kBroadcastPort));

            _receiveThread = new Thread(() => Receive());
            _receiveThread.Start();

            _mainThread = new Thread(() => MainLoop());
            _mainThread.Start();

            GetMyID();

            GetNetwork();

            PrintNeighbours();

            _networkGraph.Print();
        }

        public void SendBroadcast(Packet p)
        {
            foreach (IPEndPoint ipep in _broadcast.Values)
            {
                byte[] data = p.GetBytes();
                _udpSend.Send(data, data.Length, ipep);
            }
        }

        public void Receive()
        {
            while (true)
            {
                byte[] bytes = _udpReceive.Receive(ref _receiveEndPoint);
                Packet receivedPacket = new Packet(bytes);                

                // Do not Process Packets if they are not coming from 
                // Physical Neighbours
                if (!_neighbourNodes.Contains(receivedPacket.NodeSource))
                {
                    continue;
                }

                // Send to Next Node Hop
                if (_id == receivedPacket.NodeDestination)
                {
                    // My Message

                    receivedPacket.PrintDebugInfo();
                }
                else if (_id == receivedPacket.NodeNextHop)
                {
                    // Prepare Message to Destination
                    _idCount++;
                    Packet newPacket = new Packet()
                    {
                        NodeOriginalSource = receivedPacket.NodeOriginalSource,
                        NodeSource = _id,
                        NodeDestination = receivedPacket.NodeDestination,
                        Info = receivedPacket.Info,
                        NodeSourceCount = _idCount,
                        NodeNextHop = _networkGraph.GetNextNode(_id, receivedPacket.NodeDestination)
                    };

                    // Send Message
                    _packetQueue.Enqueue(newPacket);
                }
            }
        }

        public void MainLoop()
        {
            while (true)
            {
                Packet p;
                if (!_packetQueue.TryDequeue(out p))
                {
                    Thread.Sleep(100);
                    continue;
                }

                // Send Packet
                SendBroadcast(p);
            }
        }

        public void PrintNeighbours()
        {
            Console.Write("My Neighbours are : ");
            foreach (int n in _neighbourNodes)
            {
                Console.Write(n.ToString() + " ");
            }
            Console.WriteLine("");
        }

        public void GetMyID()
        {
            var idFile = File.ReadAllLines("ID.txt");
            _id = int.Parse(idFile[0]);
            Console.WriteLine("My ID is: " + _id.ToString());
        }

        public void GetNetwork()
        {
            var networkFile = File.ReadAllLines("network.txt");
            foreach (string pair in networkFile)
            {
                string[] p = pair.Split(',');
                int source = int.Parse(p[0]);
                int dest = int.Parse(p[1]);

                if (_id == source)
                {
                    _neighbourNodes.Add(dest);
                }

                _networkGraph[source, dest] = true;
            }
        }

        public void Foo(int info)
        {
            if (_id == 4) return;
            
            _idCount++;

            Packet p = new Packet()
            {
                Info = info,
                NodeSource = _id,
                NodeOriginalSource = _id,
                NodeDestination = 4,
                NodeNextHop = _networkGraph.GetNextNode(_id, 4),
                NodeSourceCount = _idCount
            };

            // Send Packet
            SendBroadcast(p);
        }
    }
}
