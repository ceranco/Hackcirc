﻿using System;
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
        private static int kTimeoutInSeconds = 5;
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
        List<int> _neighbourNodes = new List<int>();
        ConcurrentQueue<Packet> _packetQueueToSend = new ConcurrentQueue<Packet>();

        List<Packet> _packetListForAcknoledge = new List<Packet>();
        object mutex = new object();

        Dictionary<int, Int64> _previousSeenPackets
            = new Dictionary<int, Int64>();

        private bool _saveImage = false;
        private int _imageSize = 0;
        #endregion

        #region Properties
        public int Id { get => _id; set => _id = value; }
        #endregion

        public Node(int imgSize, bool saveImg)
        {
            _imageSize = imgSize;
            _saveImage = saveImg;

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

        private Packet PreparePacket(Packet receivedPacket)
        {
            Packet newPacket = new Packet()
            {
                NodeOriginalSource = receivedPacket.NodeOriginalSource,
                NodeSource = _id,
                NodeDestination = receivedPacket.NodeDestination,                
                InfoSize = receivedPacket.InfoSize,
                NodeOriginalSourceCount = receivedPacket.NodeOriginalSourceCount,
                IsAcknoledgment = receivedPacket.IsAcknoledgment,
                AcknoledgmentCount = receivedPacket.AcknoledgmentCount
            };

            newPacket.Info = new byte[newPacket.InfoSize];
            Buffer.BlockCopy(receivedPacket.Info, 0, newPacket.Info, 0, newPacket.InfoSize);

            return newPacket;
        }

        public void Receive()
        {
            int startIndex = 0;
            byte[] pictureStream = null;

            if (_saveImage)
            {
                pictureStream = new byte[_imageSize];
                for (int i = 0; i < _imageSize; i++)
                {
                    pictureStream[i] = 0xFF;
                }
            }

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

                // Check if Current Message is Broadcast Message
                // Send Packet To All Neighbours
                if (receivedPacket.NodeDestination == -1)
                {                    
                    // Prepare Packet
                    Packet newPacket = PreparePacket(receivedPacket);

                    // Enqueue Packet for Send
                    _packetQueueToSend.Enqueue(newPacket);

                    newPacket.PrintBroadcastInfo();
                }                
                else if (_id == receivedPacket.NodeDestination)
                {
                    // My Message                   

                    // This Package is not Acknoledgment
                    if (receivedPacket.IsAcknoledgment != 1)
                    {
                        if (_saveImage)
                        {
                            Buffer.BlockCopy(receivedPacket.Info, 0,
                                pictureStream, startIndex, receivedPacket.InfoSize);
                            //receivedPacket.PrintDebugInfo(pictureStream);
                            using (FileStream file =
                                new FileStream("picture.bmp",
                                    FileMode.Open, System.IO.FileAccess.Write))
                            {
                                file.Write(pictureStream, 0, pictureStream.Length);
                            }
                            startIndex += receivedPacket.InfoSize;
                        }

                        #region Prepare Acknowledge Packet
                        Int64 timeCount = Utility.GetTimeCount();
                        Packet newPacket = new Packet()
                        {
                            NodeOriginalSource = _id,
                            NodeSource = _id,
                            NodeDestination = receivedPacket.NodeOriginalSource,
                            IsAcknoledgment = 1,
                            NodeOriginalSourceCount = timeCount,
                            AcknoledgmentCount = receivedPacket.NodeOriginalSourceCount,
                            InfoSize = 4
                        };
                        newPacket.Info = new byte[newPacket.InfoSize];

                        // Enqueue Packet for Send
                        _packetQueueToSend.Enqueue(newPacket);

                        newPacket.PrintAcknoledgmentInfo();
                        #endregion
                    }
                    else // Acknowledge
                    {
                        // Remove from List
                        lock (mutex)
                        {
                            Packet founded = null;
                            foreach (Packet p in _packetListForAcknoledge)
                            {
                                if (p.NodeOriginalSourceCount == 
                                    receivedPacket.AcknoledgmentCount)
                                {
                                    founded = p;
                                    break;
                                }
                            }
                            if (founded != null)
                            {
                                _packetListForAcknoledge.Remove(founded);
                            }
                        }
                    }
                }
                else // Send to Next Node Hop
                {
                    // Prepare Packet
                    Packet newPacket = PreparePacket(receivedPacket);

                    // Enqueue Packet for Send
                    _packetQueueToSend.Enqueue(newPacket);

                    newPacket.PrintRelayInfo();
                }
            }
        }

        public void MainLoop()
        {
            while (true)
            {
                Packet p;
                if (_packetQueueToSend.TryDequeue(out p))
                {
                    // Send Packet
                    SendBroadcast(p);

                    // Add Packet for Acknowledge List Only 
                    // if you are an Original Source
                    // and this is not an acknowledgment packet
                    // No acknowledgment for Broadcast
                    if ((p.NodeOriginalSource == _id) && 
                        (p.IsAcknoledgment == 0) &&
                        (p.NodeDestination != -1))
                    {
                        _packetListForAcknoledge.Add(p);
                    }
                }

                lock (mutex)
                {
                    Int64 currentTimeCount = Utility.GetTimeCount();
                    List<Packet> packetsToRetransmit = new List<Packet>();
                    foreach (Packet pack in _packetListForAcknoledge)
                    {
                        if (pack.NodeOriginalSourceCount +
                            1000 * kTimeoutInSeconds < currentTimeCount)
                        {
                            packetsToRetransmit.Add(pack);
                        }
                    }

                    foreach (Packet pret in packetsToRetransmit)
                    {                        
                        pret.NodeOriginalSourceCount = currentTimeCount;

                        // Add myself to seenMessages
                        _previousSeenPackets[_id] = currentTimeCount;

                        SendBroadcast(pret);                        
                    }
                }

                Thread.Sleep(50);                
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

        public void SendInfo(byte [] info, int infoLength, int nodeDest)
        {
            if (_id != 1) return;
            
            Int64 timeCount = Utility.GetTimeCount();

            Packet p = new Packet()
            {                
                InfoSize = infoLength,
                NodeSource = _id,
                NodeOriginalSource = _id,
                NodeDestination = nodeDest,
                NodeOriginalSourceCount = timeCount
            };
            p.Info = new byte[p.InfoSize];
            Buffer.BlockCopy(info, 0, p.Info, 0, infoLength);


            // Add myself to seenMessages
            _previousSeenPackets[_id] = timeCount;

            // Enqueue Packet for Send
            _packetQueueToSend.Enqueue(p);
        }
    }
}
