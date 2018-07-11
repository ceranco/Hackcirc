using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Test
{
    public class Node
    {
        #region Constants
        private static int kBroadcastPort = 2000;
        private static int kNumNodes = 5;
        #endregion

        #region Members
        private UdpClient udpSend = new UdpClient() { EnableBroadcast = true };
        private UdpClient udpReceive = new UdpClient(kBroadcastPort) { EnableBroadcast = true };
        private Dictionary<int, IPEndPoint> broadcast = 
            new Dictionary<int, IPEndPoint>();
        IPEndPoint endpointReceive = new IPEndPoint(IPAddress.Any, 0);
        Thread receiveThread = null;
        Thread mainThread = null;
        private int myNodeID = 0;
        private Int64 myNodeIDCount = 0;
        private List<int> neighbourNodes = new List<int>();
        //private bool[,] networkMap = new bool[kNumNodes,kNumNodes];
        private NetworkGraph networkGraph = new NetworkGraph(kNumNodes);
        private ConcurrentQueue<Packet> packetQueue = new ConcurrentQueue<Packet>();
        #endregion

        public Node()
        {
            broadcast.Add(0, new IPEndPoint(IPAddress.Parse("1.1.0.29"), kBroadcastPort));
            broadcast.Add(1, new IPEndPoint(IPAddress.Parse("1.1.0.28"), kBroadcastPort));
            broadcast.Add(2, new IPEndPoint(IPAddress.Parse("1.1.0.35"), kBroadcastPort));
            broadcast.Add(3, new IPEndPoint(IPAddress.Parse("1.1.0.37"), kBroadcastPort));
            broadcast.Add(4, new IPEndPoint(IPAddress.Parse("1.1.0.64"), kBroadcastPort));

            receiveThread = new Thread(() => Receive());
            receiveThread.Start();

            mainThread = new Thread(() => MainLoop());
            mainThread.Start();

            GetMyID();

            GetNetwork();

            PrintNeighboards();

            networkGraph.Print();
        }

        public void SendBroadcast(Packet p)
        {
            foreach (IPEndPoint ipep in broadcast.Values)
            {
                byte[] data = p.GetBytes();
                udpSend.Send(data, data.Length, ipep);
            }            
        }

        public void Receive()
        {
            while (true)
            {
                byte[] bytes = udpReceive.Receive(ref endpointReceive);
                Packet receivedPacket = new Packet(bytes);
                receivedPacket.PrintDebugInfo();

                // Send to Next Node Hop
                if (myNodeID == receivedPacket.NodeDestination)
                {
                    // My Message
                }
                else if (myNodeID == receivedPacket.NodeNextHop)
                {
                    // Prepare Message to Destination
                    myNodeIDCount++;
                    Packet newPacket = new Packet()
                    {
                        NodeOriginalSource = receivedPacket.NodeOriginalSource,
                        NodeSource = myNodeID,
                        NodeDestination = receivedPacket.NodeDestination,
                        Info = receivedPacket.Info,
                        NodeSourceCount = myNodeIDCount,
                        NodeNextHop = 
                            networkGraph.GetNextNode(myNodeID, receivedPacket.NodeDestination)
                    };

                    // Send Message
                    packetQueue.Enqueue(newPacket);
                }
            }
        }

        public void MainLoop()
        {
            while(true)
            {
                Packet p;
                if (!packetQueue.TryDequeue(out p))
                {
                    Thread.Sleep(100);
                    continue;
                }

                // Send Packet
                SendBroadcast(p);
            }
        }        

        public void PrintNeighboards()
        {
            Console.Write("My Neighbours are : ");
            foreach (int n in neighbourNodes)
            {
                Console.Write(n.ToString() + " ");
            }
            Console.WriteLine("");
        }

        public void GetMyID()
        {
            var idFile = File.ReadAllLines("ID.txt");
            myNodeID = int.Parse(idFile[0]);
            Console.WriteLine("My ID is: " + myNodeID.ToString());
        }

        public void GetNetwork()
        {
            var networkFile = File.ReadAllLines("network.txt");
            foreach (string pair in networkFile)
            {
                string[] p = pair.Split(',');
                int source = int.Parse(p[0]);
                int dest = int.Parse(p[1]);

                if (myNodeID == source)
                {
                    neighbourNodes.Add(dest);
                }
                
                networkGraph[source, dest] = true;
            }
        }

        public void Foo(int info)
        {
            myNodeIDCount++;

            Packet p = new Packet()
            {
                 Info = info,
                  NodeSource = myNodeID,
                   NodeOriginalSource = myNodeID,
                    NodeDestination = 4,
                     NodeNextHop = networkGraph.GetNextNode(myNodeID, 4),
                      NodeSourceCount = myNodeIDCount
            };
        }
    }
}
