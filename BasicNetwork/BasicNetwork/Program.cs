
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
            Console.WriteLine("Program Version 2.18\n");

            bool imageExists = false;
            if (File.Exists("BW.bmp")) imageExists = true;

            byte[] bmpBytes = new byte[100];

            Node node = null;

            if (imageExists)
            {
                //Bitmap bmp = new Bitmap("BW.bmp");
                Bitmap bmp = new Bitmap("BW.bmp");

                MemoryStream ms = new MemoryStream();
                // Save to memory using the BMP format
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);

                // read to end
                bmpBytes = ms.GetBuffer();
                bmp.Dispose();
                ms.Close();

                node = new Node(bmpBytes.Length, true);
            }
            else
            {
                node = new Node(0, false);
            }

            if (node.Id == 1)
            {                
                int size = 200;
                int index = 0;
                while (index <= bmpBytes.Length)
                {
                    byte[] result = new byte[size];

                    int length = Math.Min((bmpBytes.Length - index), result.Length);

                    Buffer.BlockCopy(bmpBytes, index,
                        result, 0, length);
                    index += size;

                    node.SendInfo(result, length, 0);

                    Thread.Sleep(1000);
                }
                Console.WriteLine("\nFinished Sending The Picture\n");
            }

            while(true)
            {
                Thread.Sleep(10000);
            }
        }
    }
}
