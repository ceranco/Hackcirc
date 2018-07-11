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
        //private static int kNumNodes = 5;
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
        //Int64 _idCount = 0;
        List<int> _neighbourNodes = new List<int>();
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
        }

        public void SendBroadcast(Packet p)
        {
            for (int i = 0; i < _broadcast.Count; i++)
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

                    Console.WriteLine("Relay Src->Dest {0}->{1}, OrigSrcCnt {2}",
                        newPacket.NodeOriginalSource,
                        newPacket.NodeDestination,
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
            var neighbours = File.ReadAllLines("NB.txt");
            foreach (string n in neighbours)
            {
                _neighbourNodes.Add(int.Parse(n));
            }
        }

        public void Foo(int info)
        {
            if (_id != 1) return;

            // Time since 1970, in order 
            var idCount = DateTime.Now.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;

            Packet p = new Packet()
            {
                Info = info,
                NodeSource = _id,
                NodeOriginalSource = _id,
                NodeDestination = 4,
                NodeOriginalSourceCount = idCount
            };

            // Add myself to seenMessages
            _previousSeenPackets[_id] = idCount;

            // Send Packet
            SendBroadcast(p);
        }
    }
}
