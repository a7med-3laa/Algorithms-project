using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;

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
                    char[] bits = huffmanRed.ColorsMap[ImageMatrix[i, j].red].Value.ToCharArray();
                    for (int k = 0; k < bits.Length; k++)
                    {
                        if (bits[k] == '1')
                            tempRed.Add(true);
                        else
                            tempRed.Add(false);
                    }
                    bits = huffmanGreen.ColorsMap[ImageMatrix[i, j].green].Value.ToCharArray();
                    for (int k = 0; k < bits.Length; k++)
                    {
                        if (bits[k] == '1')
                            tempGreen.Add(true);
                        else
                            tempGreen.Add(false);
                    }
                    bits = huffmanBlue.ColorsMap[ImageMatrix[i, j].blue].Value.ToCharArray();
                    for (int k = 0; k < bits.Length; k++)
                    {
                        if (bits[k] == '1')
                            tempBlue.Add(true);
                        else
                            tempBlue.Add(false);
                    }
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
