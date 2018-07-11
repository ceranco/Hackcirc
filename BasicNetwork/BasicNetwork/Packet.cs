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
        public Int64 NodeOriginalSourceCount { get; set; }
        public int NodeDestination { get; set; }
        public int Info { get; set; }
        #endregion

        public Packet(byte[] data)
        {
            int index = 0;
            NodeOriginalSource = BitConverter.ToInt32(data, index);
            index += sizeof(int);
            NodeSource = BitConverter.ToInt32(data, index);
            index += sizeof(int);
            NodeOriginalSourceCount = BitConverter.ToInt32(data, index);
            index += sizeof(int);
            NodeDestination = BitConverter.ToInt32(data, index);
            index += sizeof(int);
            Info = BitConverter.ToInt32(data, index);
        }

        public Packet()
        {
        }

        public void PrintDebugInfo()
        {
            Console.WriteLine("NodeOriginalSource {0}, NodeSource {1}, NodeOriginalSourceCount {2},NodeDestination {3}, Info {4}",
                NodeOriginalSource, NodeSource, NodeOriginalSourceCount, NodeDestination, Info);
        }

        public byte[] GetBytes()
        {
            IEnumerable<byte> result = Enumerable.Empty<byte>();

            result = result.Concat(BitConverter.GetBytes(NodeOriginalSource));
            result = result.Concat(BitConverter.GetBytes(NodeSource));
            result = result.Concat(BitConverter.GetBytes(NodeOriginalSourceCount));
            result = result.Concat(BitConverter.GetBytes(NodeDestination));
            result = result.Concat(BitConverter.GetBytes(Info));

            byte[] array = result.ToArray();

            return array;
        }
    }
}
