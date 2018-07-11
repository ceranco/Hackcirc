using System;
using System.Collections.Generic;
using System.Linq;

namespace Node
{
    internal class Packet
    {
        #region Properties
        public int NodeOriginalSource { get; set; }
        public int NodeSource { get; set; }
        public Int64 NodeOriginalSourceCount { get; set; } // Unique ID
        public int NodeDestination { get; set; } // -1 Means All
        public int IsAcknoledgment { get; set; }
        public Int64 AcknoledgmentCount { get; set; }
        public int InfoSize { get; set; }
        public byte[] Info { get; set; }
        #endregion

        public Packet(byte[] data)
        {
            int index = 0;
            NodeOriginalSource = BitConverter.ToInt32(data, index);
            index += sizeof(int);
            NodeSource = BitConverter.ToInt32(data, index);
            index += sizeof(int);
            NodeOriginalSourceCount = BitConverter.ToInt64(data, index);
            index += sizeof(Int64);
            NodeDestination = BitConverter.ToInt32(data, index);
            index += sizeof(int);
            IsAcknoledgment = BitConverter.ToInt32(data, index);
            index += sizeof(int);
            AcknoledgmentCount = BitConverter.ToInt64(data, index);
            index += sizeof(Int64);
            InfoSize = BitConverter.ToInt32(data, index);
            index += sizeof(int);
            Info = new byte[InfoSize];
            Array.Copy(data, index, Info, 0, InfoSize);
        }

        public Packet()
        {
        }

        public void PrintDebugInfo()
        {
            Console.WriteLine("NodeOrigSrc {0}, NodeSrc {1}, NodeOrigSrcCnt {2}, NodeDest {3}, IsAckt {4}, AckCnt {5}",
                NodeOriginalSource, NodeSource,
                NodeOriginalSourceCount, NodeDestination,
                IsAcknoledgment, AcknoledgmentCount);

            //int[] result = new int[Info.Length / 4];
            //for (int i = 0; i < Info.Length; i += 4)
            //{
            //    result[i / 4] = BitConverter.ToInt32(Info, i);
            //}

            //using (System.IO.StreamWriter file =
            //    new System.IO.StreamWriter(@"output.txt"))
            //{
            //    foreach (int num in result)
            //    {
            //        file.WriteLine(num);                    
            //    }
            //}            
        }

        public void PrintBroadcastInfo()
        {
            Console.WriteLine("BroadCast Src {0}, OrigSrcCnt {1}",
                        NodeOriginalSource,
                        NodeOriginalSourceCount);
        }

        public void PrintRelayInfo()
        {
            Console.WriteLine("Relay Src->Dest {0}->{1}, OrigSrcCnt {2}",
                        NodeOriginalSource,
                        NodeDestination,
                        NodeOriginalSourceCount);
        }

        public void PrintAcknoledgmentInfo()
        {
            Console.WriteLine("Ack Src->Dest {0}->{1}, OrigSrcCnt {2}",
                        NodeOriginalSource,
                        NodeDestination,
                        NodeOriginalSourceCount);
        }

        public byte[] GetBytes()
        {
            IEnumerable<byte> result = Enumerable.Empty<byte>();

            result = result.Concat(BitConverter.GetBytes(NodeOriginalSource));
            result = result.Concat(BitConverter.GetBytes(NodeSource));
            result = result.Concat(BitConverter.GetBytes(NodeOriginalSourceCount));
            result = result.Concat(BitConverter.GetBytes(NodeDestination));

            result = result.Concat(BitConverter.GetBytes(IsAcknoledgment));
            result = result.Concat(BitConverter.GetBytes(AcknoledgmentCount));
            result = result.Concat(BitConverter.GetBytes(InfoSize));
            result = result.Concat(Info);

            byte[] array = result.ToArray();

            return array;
        }
    }
}

