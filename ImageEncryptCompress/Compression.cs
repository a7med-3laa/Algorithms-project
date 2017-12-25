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
        static int count = 0;
        public static void compress(RGBPixel[,] ImageMatrix, Huffman huffmanRed, Huffman huffmanGreen, Huffman huffmanBlue,string seed,short tap)
        {
            List<bool> tempRed = new List<bool>();
            List<bool> tempBlue = new List<bool>();
            List<bool> tempGreen = new List<bool>();
            int count = 0;
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
            byte[] RedTree = Encoding.ASCII.GetBytes(huffmanRed.writeHuffman());
            byte[] greenTree = Encoding.ASCII.GetBytes(huffmanGreen.writeHuffman());
            byte[] blueTree = Encoding.ASCII.GetBytes(huffmanBlue.writeHuffman());
            byte[] redTreeLength = BitConverter.GetBytes(RedTree.Length);
            byte[] greenTreeLength = BitConverter.GetBytes(greenTree.Length);
            byte[] blueTreeLength = BitConverter.GetBytes(blueTree.Length);
            byte[] width = BitConverter.GetBytes(ImageMatrix.GetLength(0));
            byte[] hight = BitConverter.GetBytes(ImageMatrix.GetLength(1));
            byte[] seed2Length = BitConverter.GetBytes(seed.Length);
            byte[] seed2 = Encoding.ASCII.GetBytes(seed);
            byte[] tap1 = BitConverter.GetBytes(tap);
            
            int fileLength = (RedTree.Length) + greenTree.Length + blueTree.Length +
                redTreeLength.Length + greenTreeLength.Length + blueTreeLength.Length +
                width.Length + hight.Length + seed2.Length + tap1.Length + 4;

            List<Byte> bytes2 = new List<Byte>(fileLength + (tempRed.Count / 8 + (tempRed.Count % 8 == 0 ? 0 : 1)));
            bytes2.AddRange(redTreeLength);
            bytes2.AddRange(RedTree);
            bytes2.AddRange(greenTreeLength);
            bytes2.AddRange(greenTree);
            bytes2.AddRange(blueTreeLength);
            bytes2.AddRange(blueTree);
            bytes2.AddRange(width);
            bytes2.AddRange(hight);
            bytes2.AddRange(seed2Length);
            bytes2.AddRange(seed2);
            bytes2.AddRange(tap1);
            
            Byte[] bytes = new byte[tempRed.Count / 8 + (tempRed.Count % 8 == 0 ? 0 : 1)];
            BitArray d = new BitArray(tempRed.ToArray());
            d.CopyTo(bytes, 0);
            bytes2.AddRange(bytes);
            File.WriteAllBytes(fileName, bytes2.ToArray());
        }

        public static RGBPixel[,] decompress(string path)
        {
            int fileoffset;
            byte[] fileData=  File.ReadAllBytes(path);
            int RedTreesize = BitConverter.ToInt32(fileData, 0);
            Huffman huffmanRed = new Huffman(getPriorityQueue(Encoding.ASCII.GetString(fileData,4,RedTreesize)));

            int greenTreesize = BitConverter.ToInt32(fileData, RedTreesize + 4);
            Huffman huffmanGreen = new Huffman(getPriorityQueue(Encoding.ASCII.GetString(fileData, 8 + RedTreesize, greenTreesize)));

            int blueTreesize = BitConverter.ToInt32(fileData, 8 + greenTreesize + RedTreesize) ;
            Huffman huffmanBlue = new Huffman(getPriorityQueue(Encoding.ASCII.GetString(fileData, 12 + greenTreesize + RedTreesize, blueTreesize)));
            fileoffset = RedTreesize + blueTreesize + greenTreesize + 12;
            int width = BitConverter.ToInt32(fileData, fileoffset);
            fileoffset += 4;
            int height= BitConverter.ToInt32(fileData, fileoffset);
            fileoffset += 4;
            int seedLength = BitConverter.ToInt32(fileData, fileoffset);
            fileoffset += 4;
            string seed = Encoding.ASCII.GetString(fileData,fileoffset,seedLength);
            fileoffset +=seedLength;
            int tap = (int)BitConverter.ToInt16(fileData, fileoffset);
            fileoffset += 2;
            
            byte[] bin = new byte[fileData.Length - fileoffset+1];
            Array.Copy(fileData, fileoffset, bin, 0,fileData.Length - fileoffset);
            BitArray b = new BitArray(bin);
            Boolean[] binary = new Boolean[b.Length];
            b.CopyTo(binary,0);
            
            //loop for red color
            count = 0;
            RGBPixel[,] Image = new RGBPixel[width, height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    Image[i, j].red = getColor(huffmanRed.start, binary);
                }
            }
            // loop for Green color
             for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    Image[i, j].green = getColor(huffmanGreen.start, binary);
                }
            }
             
            // loop for Blue color
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    Image[i, j].blue = getColor(huffmanBlue.start, binary);
                }
            }
         ImageOperations.encrypt(Image, tap, seed);
            return Image;
        }

        private static byte getColor(Node start,  Boolean[] s)
        {
            Node n = start;
            while (n.hasChildreen())
            {     
                if (s[count]==false)
                    n = n.left;
                else
                    n = n.right;
                count++;
            }
            return n.color;
        }
        public static PriorityQueue getPriorityQueue(string s)
        {
            s = s.Substring(1, s.Length - 2);
           PriorityQueue tempDic = new PriorityQueue();
            string[] temp = s.Split(',');
            for (int i = 0; i < temp.Length; i++)
            {   int color = Int32.Parse(temp[i]);
                i++;
                int freq = Int32.Parse(temp[i]);
                  tempDic.Enqueue(new Node((byte)color,freq));         
            }
            return tempDic;

        }

    }
}
