
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BasicNetwork
{
    class Program
    {
        static void Main(string[] args)
        {
            int count = 0;
            Node node = new Node();
            //string str = "Is anyone out there?";

            while (true)
            {
                count++;

                //string str = count.ToString();
                //byte[] sendBytes = Encoding.ASCII.GetBytes(str);
                //Message m = new Message(Encoding.ASCII.GetBytes(str));
                //node.SendBroadcast(m);

                node.Foo(count);
                Thread.Sleep(1000);
            }
        }
    }
}
