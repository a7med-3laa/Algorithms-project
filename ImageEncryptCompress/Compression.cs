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
        static int countR = 0;
        static int countG = 0;
        static int countB = 0; static int countBoolean = 0;


        public static void compress(RGBPixel[,] ImageMatrix, Huffman huffmanRed, Huffman huffmanGreen, Huffman huffmanBlue, string seed, short tap)
        {
            List<bool> tempRed = new List<bool>(ImageMatrix.GetLength(0)* ImageMatrix.GetLength(1)*3);
            List<bool> tempBlue = new List<bool>();
            List<bool> tempGreen = new List<bool>();
            BinaryWriter b = new BinaryWriter(File.Open(fileName, FileMode.Create));
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
            byte[] redL = BitConverter.GetBytes(tempRed.Count);
            byte[] greenL = BitConverter.GetBytes(tempGreen.Count);
            byte[] blueL = BitConverter.GetBytes(tempBlue.Count);

            int fileLength = (RedTree.Length) + greenTree.Length + blueTree.Length +
                redTreeLength.Length + greenTreeLength.Length + blueTreeLength.Length +
                width.Length + hight.Length + seed2.Length + tap1.Length + seed2Length.Length+redL.Length+greenL.Length
                +blueL.Length;

            List<Byte> bytes2 = new List<Byte>(fileLength);
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
            bytes2.AddRange(redL);
            bytes2.AddRange(greenL);
            bytes2.AddRange(blueL);

            Byte[] bytes = new byte[tempRed.Count / 8 + (tempRed.Count % 8 == 0 ? 0 : 1)];
            BitArray d = new BitArray(tempRed.ToArray());
            d.CopyTo(bytes, 0);
            b.Write(bytes2.ToArray());
            b.Write(bytes.ToArray());
            b.Close();
        }

        public static RGBPixel[,] decompress(string path, RGBPixel[,] temp2)
        {
            countB = 0;
            countR = 0;
            countG = 0;
            int fileoffset;
            byte[] fileData = File.ReadAllBytes(path);
            int RedTreesize = BitConverter.ToInt32(fileData, 0);
            Huffman huffmanRed = new Huffman(getPriorityQueue(Encoding.ASCII.GetString(fileData, 4, RedTreesize)));

            int greenTreesize = BitConverter.ToInt32(fileData, RedTreesize + 4);
            Huffman huffmanGreen = new Huffman(getPriorityQueue(Encoding.ASCII.GetString(fileData, 8 + RedTreesize, greenTreesize)));

            int blueTreesize = BitConverter.ToInt32(fileData, 8 + greenTreesize + RedTreesize);
            Huffman huffmanBlue = new Huffman(getPriorityQueue(Encoding.ASCII.GetString(fileData, 12 + greenTreesize + RedTreesize, blueTreesize)));
            fileoffset = RedTreesize + blueTreesize + greenTreesize + 12;
            int width = BitConverter.ToInt32(fileData, fileoffset);
            fileoffset += 4;
            int height = BitConverter.ToInt32(fileData, fileoffset);
            fileoffset += 4;
            int seedLength = BitConverter.ToInt32(fileData, fileoffset);
            fileoffset += 4;
            string seed = Encoding.ASCII.GetString(fileData, fileoffset, seedLength);
            fileoffset += seedLength;
            int tap = (int)BitConverter.ToInt16(fileData, fileoffset);
            fileoffset += 2;
            int redL = BitConverter.ToInt32(fileData, fileoffset);
            fileoffset += 4;
            int greenL = BitConverter.ToInt32(fileData, fileoffset);
            fileoffset += 4;
            int blueL = BitConverter.ToInt32(fileData, fileoffset);
            fileoffset += 4;
            redL -= greenL;
            redL -= blueL;

            byte[] binR = new byte[fileData.Length - fileoffset];
            Array.Copy(fileData, fileoffset, binR, 0, fileData.Length - fileoffset);

            BitArray bR = new BitArray(binR);

            countBoolean = 0;
            Boolean[] binaryR = new Boolean[redL];
            feedBooleanArray(bR, 0, binaryR, redL);

            Boolean[] binaryG = new Boolean[greenL];
            countBoolean = 0;
            feedBooleanArray(bR, redL, binaryG, greenL);

            Boolean[] binaryB = new Boolean[blueL];
            countBoolean = 0;
            feedBooleanArray(bR, redL + greenL, binaryB, blueL);

            RGBPixel[,] Image = new RGBPixel[width, height];

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    Image[i, j].red = getColor(huffmanRed.start, binaryR, ref countR);
                    Image[i, j].green = getColor(huffmanGreen.start, binaryG, ref countG);
                    Image[i, j].blue = getColor(huffmanBlue.start, binaryB, ref countB);

                }
            }

            ImageOperations.encrypt(Image, tap, seed);
            return Image;
        }

        private static byte getColor(Node start, Boolean[] s, ref int count)
        {
            Node n = start;
            while (n.hasChildreen())
            {
                if (s[count] == false)
                    n = n.left;
                else
                    n = n.right;
                count++;
            }

            return n.color;
        }
        public static PriorityQueue getPriorityQueue(string s)
        {
            PriorityQueue tempDic = new PriorityQueue();
            string[] temp = s.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < temp.Length; i++)
            {
                int color = Int32.Parse(temp[i]);
                i++;
                int freq = Int32.Parse(temp[i]);
                tempDic.Enqueue(new Node((byte)color, freq));
            }
            return tempDic;

        }
      
        public static void feedBooleanArray(BitArray bt, int startIndex ,Boolean[] color,int length)
        {   for (int i = startIndex; i < startIndex+length; i++)
            {

                color[countBoolean++] = bt.Get(i);
             

            }

        }

    }
}