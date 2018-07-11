
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
            Console.WriteLine("Program Version 1.0.7\n");

            int count = 0;
            Node node = new Node();
            
            while (true)
            {
                count++;
                node.Foo(count);
                Thread.Sleep(10000);
            }
        }
    }
}
