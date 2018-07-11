
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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
            Console.WriteLine("Program Version 2.16\n");

            Node node = new Node();

            //int[] intArray = new int[256];
            //for (int i =0; i < 256; i ++)
            //{
            //    intArray[i] = i;
            //}

            //byte[] result = new byte[intArray.Length * sizeof(int)];
            //Buffer.BlockCopy(intArray, 0, result, 0, result.Length);

            //node.Foo(result, result.Length);

            Bitmap bmp = new Bitmap("BW.bmp");

            MemoryStream ms = new MemoryStream();
            // Save to memory using the BMP format
            bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);

            // read to end
            byte[] bmpBytes = ms.GetBuffer();
            bmp.Dispose();
            ms.Close();

            int size = 200;
            int index = 0;
            while (index <= bmpBytes.Length)
            {
                byte[] result = new byte[size];

                int length = Math.Min((bmpBytes.Length - index), result.Length);

                Buffer.BlockCopy(bmpBytes, index, 
                    result, 0, length);
                index += size;
                node.Foo(result, length);
                Thread.Sleep(1000);
            }
            Console.WriteLine("\nFinished Sending The Picture\n");
            while(true)
            {
                Thread.Sleep(10000);
            }
        }
    }
}
