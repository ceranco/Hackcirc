using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    public class Message
    {
        private byte[] data = null;

        public byte[] Data { get => data; set => data = value; }

        public Message(byte[] _data)
        {
            data = _data;
        }        
    }
}
