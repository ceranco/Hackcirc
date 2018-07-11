
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
            Console.WriteLine("Program Version 2.9\n");

            Node node = new Node();

            int[] intArray = new int[256];
            for (int i =0; i < 256; i ++)
            {
                intArray[i] = i;
            }

            byte[] result = new byte[intArray.Length * sizeof(int)];
            Buffer.BlockCopy(intArray, 0, result, 0, result.Length);

            node.Foo(result);

            while (true)
            {
                //count++;
                //node.Foo(count);
                Thread.Sleep(15000);
            }
        }
    }
}
