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
        public static void compress(RGBPixel[,] ImageMatrix, Huffman huffmanRed, Huffman huffmanGreen, Huffman huffmanBlue,string Initialseed,short tap)
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

            byte[] RedTree = Encoding.ASCII.GetBytes(huffmanRed.writeHuffman());
            byte[] greenTree = Encoding.ASCII.GetBytes(huffmanGreen.writeHuffman());
            byte[] blueTree = Encoding.ASCII.GetBytes(huffmanBlue.writeHuffman());
            byte[] redTreeLength=BitConverter.GetBytes(RedTree.Length);
            byte[] greenTreeLength = BitConverter.GetBytes(greenTree.Length);
            byte[] blueTreeLength = BitConverter.GetBytes(blueTree.Length);
            byte[] width = BitConverter.GetBytes(ImageMatrix.GetLength(0));
            byte[] hight = BitConverter.GetBytes(ImageMatrix.GetLength(1));
            byte[] seed = Encoding.ASCII.GetBytes(Initialseed);
            byte[] tap1 = BitConverter.GetBytes(tap);
            byte[] redListLength = BitConverter.GetBytes(tempRed.Count);
            byte[] blueListLength = BitConverter.GetBytes(tempGreen.Count);
            byte[] grrenListLength = BitConverter.GetBytes(tempBlue.Count);

            int fileLength = (RedTree.Length) + greenTree.Length + blueTree.Length +
                redTreeLength.Length + greenTreeLength.Length + blueTreeLength.Length +
                width.Length + hight.Length + seed.Length + tap1.Length+12;

            List<Byte> bytes2 = new List<Byte>(fileLength+(tempRed.Count / 8 + (tempRed.Count % 8 == 0 ? 0 : 1)));
            bytes2.AddRange(redTreeLength);
            bytes2.AddRange(RedTree);
            bytes2.AddRange(greenTreeLength);
            bytes2.AddRange(greenTree);
            bytes2.AddRange(blueTreeLength);
            bytes2.AddRange(blueTree);
            bytes2.AddRange(width);
            bytes2.AddRange(hight);
            bytes2.AddRange(seed);
            bytes2.AddRange(tap1);
            bytes2.AddRange(redListLength);
            bytes2.AddRange(grrenListLength);
            bytes2.AddRange(blueListLength);
            
            Byte[] bytes = new byte[tempRed.Count / 8 + (tempRed.Count % 8 == 0 ? 0 : 1)];
            
            BitArray d = new BitArray(tempRed.ToArray());
            d.CopyTo(bytes,0);
            bytes2.AddRange(bytes);

            File.WriteAllBytes(fileName, bytes2.ToArray());



        }
    }
}
