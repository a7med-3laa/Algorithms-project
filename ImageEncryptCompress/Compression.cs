using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;
using System.Diagnostics;
using System.Threading;

namespace ImageQuantization
{
    class Compression
    {
        static string fileName = "compressEncode.bin";
        public static void compress(RGBPixel[,] ImageMatrix, Huffman huffmanRed, Huffman huffmanGreen, Huffman huffmanBlue)
        {
            
           
           

            List<bool> tempRed = new List<bool>();
            List<bool> tempBlue = new List<bool>();
            List<bool> tempGreen = new List<bool>();
            for (int i = 0; i < ImageMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < ImageMatrix.GetLength(1); j++)
                {
                    tempRed.AddRange(huffmanRed.ColorsMap[ImageMatrix[i, j].red].Value);
                    tempGreen.AddRange(huffmanGreen.ColorsMap[ImageMatrix[i, j].green].Value);
                    tempBlue.AddRange(huffmanBlue.ColorsMap[ImageMatrix[i, j].blue].Value);
                }
            }
            tempRed.AddRange(tempGreen);
            tempRed.AddRange(tempBlue);
            byte[] bytes = new byte[tempRed.Count / 8 + (tempRed.Count % 8 == 0 ? 0 : 1)];
            BitArray d = new BitArray(tempRed.ToArray());
            d.CopyTo(bytes, 0);
            File.WriteAllBytes(fileName, bytes);
        }
    }
}
