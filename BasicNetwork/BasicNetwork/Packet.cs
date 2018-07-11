using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicNetwork
{
    public class Packet
    {
        #region Properties
        public int NodeOriginalSource { get; set; }
        public int NodeSource { get; set; }
        public Int64 NodeOriginalSourceCount { get; set; } // Unique ID
        public int NodeDestination { get; set; } // -1 Means All
        public int IsAcknoledgment { get; set; }
        public Int64 AcknoledgmentCount { get; set; }
        public int Info { get; set; }       
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
            Info = BitConverter.ToInt32(data, index);
        }

        public Packet()
        {
        }

        public void PrintDebugInfo()
        {
            Console.WriteLine("NodeOrigSrc {0}, NodeSrc {1}, NodeOrigSrcCnt {2}, NodeDest {3}, IsAckt {4}, AckCnt {5}, Info {6}",
                NodeOriginalSource, NodeSource, 
                NodeOriginalSourceCount, NodeDestination,
                IsAcknoledgment, AcknoledgmentCount,
                Info);
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

            result = result.Concat(BitConverter.GetBytes(Info));

            byte[] array = result.ToArray();

            return array;
        }
    }
}
