
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
            Console.WriteLine("Program Version 2.4\n");

            int count = 0;
            Node node = new Node();

            count++;
            node.Foo(count);

            while (true)
            {
                //count++;
                //node.Foo(count);
                Thread.Sleep(15000);
            }
        }
    }
}
