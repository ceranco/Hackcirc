
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
            Console.WriteLine("Program Version 2.19\n");
            string pictureName = "BW.bmp";
            //string pictureName = "team2.bmp";

            bool imageExists = false;
            if (File.Exists(pictureName)) imageExists = true;

            byte[] bmpBytes = new byte[100];

            Node node = null;

            if (imageExists)
            {
                Bitmap bmp = new Bitmap(pictureName);

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
                int size = 100;
                int index = 0;
                Console.WriteLine("\nTransferring Image Started\n");
                while (index <= bmpBytes.Length)
                {
                    byte[] result = new byte[size];

                    int length = Math.Min((bmpBytes.Length - index), result.Length);

                    Buffer.BlockCopy(bmpBytes, index,
                        result, 0, length);
                    index += size;

                    node.SendInfo(result, length, 0);

                    Thread.Sleep(500);
                }
                Console.WriteLine("\nTransferring Image Completed\n");
            }

            while(true)
            {
                Thread.Sleep(10000);
            }
        }
    }
}
