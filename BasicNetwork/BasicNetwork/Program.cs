
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
            Console.WriteLine("Version 1.0.0");

            while (true)
            {
                count++;
                node.Foo(count);
                Thread.Sleep(2000);
            }
        }
    }
}
