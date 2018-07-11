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

        Dictionary<int, Int64> _previousSeenPackets
            = new Dictionary<int, Int64>();

        #endregion

        public Node()
        {
            _broadcast.Add(0, new IPEndPoint(IPAddress.Parse("1.1.0.29"), kBroadcastPort));
            _broadcast.Add(1, new IPEndPoint(IPAddress.Parse("1.1.0.28"), kBroadcastPort));
            _broadcast.Add(2, new IPEndPoint(IPAddress.Parse("1.1.0.35"), kBroadcastPort));
            _broadcast.Add(3, new IPEndPoint(IPAddress.Parse("1.1.0.37"), kBroadcastPort));
            _broadcast.Add(4, new IPEndPoint(IPAddress.Parse("1.1.0.64"), kBroadcastPort));

            _previousSeenPackets.Add(0, 0);
            _previousSeenPackets.Add(1, 0);
            _previousSeenPackets.Add(2, 0);
            _previousSeenPackets.Add(3, 0);
            _previousSeenPackets.Add(4, 0);

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
            for (int i = 0; i < _broadcast.Count; i ++)
            {
                if (_id == i) continue;
                byte[] data = p.GetBytes();
                _udpSend.Send(data, data.Length, _broadcast[i]);
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

                // Do not Process packets when they already exists 
                // In previous seen packets
                if (_previousSeenPackets[receivedPacket.NodeOriginalSource] >= 
                    receivedPacket.NodeOriginalSourceCount)
                {
                    continue;
                }

                _previousSeenPackets[receivedPacket.NodeOriginalSource] =
                    receivedPacket.NodeOriginalSourceCount;

                // Send to Next Node Hop
                if (_id == receivedPacket.NodeDestination)
                {
                    // My Message
                    receivedPacket.PrintDebugInfo();
                }
                else
                {
                    // Prepare Message to Destination
                    Packet newPacket = new Packet()
                    {
                        NodeOriginalSource = receivedPacket.NodeOriginalSource,
                        NodeSource = _id,
                        NodeDestination = receivedPacket.NodeDestination,
                        Info = receivedPacket.Info,
                        NodeOriginalSourceCount = receivedPacket.NodeOriginalSourceCount,
                    };

                    // Send Message
                    _packetQueue.Enqueue(newPacket);

                    Console.WriteLine("Relay OrigSrc {0}, OrigSrcCnt {1}",
                        newPacket.NodeOriginalSource,
                        newPacket.NodeOriginalSourceCount);
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
            Console.WriteLine("");
        }

        public void GetMyID()
        {
            var idFile = File.ReadAllLines("ID.txt");
            _id = int.Parse(idFile[0]);
            Console.WriteLine("My ID is: " + _id.ToString());
            Console.WriteLine("");
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
            if (_id != 1) return;
            
            _idCount++;

            Packet p = new Packet()
            {
                Info = info,
                NodeSource = _id,
                NodeOriginalSource = _id,
                NodeDestination = 4,
                NodeOriginalSourceCount = _idCount
            };

            // Send Packet
            SendBroadcast(p);
        }
    }
}
