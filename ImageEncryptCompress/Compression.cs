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
        static string fileName = "compressEncode.bin"; // Θ(1)
        static int countR = 0; //Θ(1)
        static int countG = 0; //Θ(1)
        static int countB = 0;//Θ(1)

        public static void compress(RGBPixel[,] ImageMatrix, Huffman huffmanRed, Huffman huffmanGreen, Huffman huffmanBlue, string seed, short tap)
        {
            List<bool> tempRed = new List<bool>(ImageMatrix.GetLength(0) * ImageMatrix.GetLength(1) * 3); //Θ(1)
            List<bool> tempBlue = new List<bool>(ImageMatrix.GetLength(0) * ImageMatrix.GetLength(1));//Θ(1)
            List<bool> tempGreen = new List<bool>(ImageMatrix.GetLength(0) * ImageMatrix.GetLength(1));//Θ(1)
            BinaryWriter b = new BinaryWriter(File.Open(fileName, FileMode.Create));//Θ(1)
            //Θ(N^2)
            for (int i = 0; i < ImageMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < ImageMatrix.GetLength(1); j++)
                {
                    tempRed.AddRange(huffmanRed.ColorsMap[ImageMatrix[i, j].red].Value); //Θ(1)
                    tempGreen.AddRange(huffmanGreen.ColorsMap[ImageMatrix[i, j].green].Value); //Θ(1)
                    tempBlue.AddRange(huffmanBlue.ColorsMap[ImageMatrix[i, j].blue].Value); //Θ(1)
                }
            }
            tempRed.AddRange(tempGreen); //Θ(N) n is size of tempGreen
            tempRed.AddRange(tempBlue); //Θ(N) n is size of tempBlue
            byte[] RedTree = Encoding.ASCII.GetBytes(huffmanRed.writeHuffman()); //Θ(N) N is length of String of "color,freq"
            byte[] greenTree = Encoding.ASCII.GetBytes(huffmanGreen.writeHuffman()); //Θ(N) N is length of String of "color,freq"
            byte[] blueTree = Encoding.ASCII.GetBytes(huffmanBlue.writeHuffman()); //Θ(N) N is length of String of "color,freq"
            byte[] redTreeLength = BitConverter.GetBytes(RedTree.Length); // length of int is 4 bytes so Θ(1)
            byte[] greenTreeLength = BitConverter.GetBytes(greenTree.Length); // length of int is 4 bytes so Θ(1)
            byte[] blueTreeLength = BitConverter.GetBytes(blueTree.Length);// length of int is 4 bytes so Θ(1)
            byte[] width = BitConverter.GetBytes(ImageMatrix.GetLength(0));// length of int is 4 bytes so Θ(1)
            byte[] hight = BitConverter.GetBytes(ImageMatrix.GetLength(1));// length of int is 4 bytes so Θ(1)
            byte[] seed2Length = BitConverter.GetBytes(seed.Length);// length of int is 4 bytes so Θ(1)
            byte[] seed2 = Encoding.ASCII.GetBytes(seed);//Θ(N) N is length of String of "color,freq"//Θ(N) N is length of String of "color,freq"
            byte[] tap1 = BitConverter.GetBytes(tap);// length of short is 2 bytes so Θ(1)
            byte[] redL = BitConverter.GetBytes(tempRed.Count);// length of int is 4 bytes so Θ(1)
            byte[] greenL = BitConverter.GetBytes(tempGreen.Count);// length of int is 4 bytes so Θ(1)
            byte[] blueL = BitConverter.GetBytes(tempBlue.Count);// length of int is 4 bytes so Θ(1)

            int fileLength = (RedTree.Length) + greenTree.Length + blueTree.Length +
                redTreeLength.Length + greenTreeLength.Length + blueTreeLength.Length +
                width.Length + hight.Length + seed2.Length + tap1.Length + seed2Length.Length + redL.Length + greenL.Length
                + blueL.Length; //Θ(1)

            List<Byte> bytes2 = new List<Byte>(fileLength);  //Θ(1)
            bytes2.AddRange(redTreeLength);     //Θ(1)  n=4 
            bytes2.AddRange(RedTree);           //Θ(N) n is size of redTree array
            bytes2.AddRange(greenTreeLength);   //Θ(1)  n=4 
            bytes2.AddRange(greenTree);         //Θ(N) n is size of greenTree array
            bytes2.AddRange(blueTreeLength);    //Θ(1)  n=4
            bytes2.AddRange(blueTree);          //Θ(N) n is size of blueTree array
            bytes2.AddRange(width);             //Θ(1)  n=4
            bytes2.AddRange(hight);             //Θ(1)  n=4
            bytes2.AddRange(seed2Length);       //Θ(N) n is size of tempGreen
            bytes2.AddRange(seed2);             //Θ(1)  n=4
            bytes2.AddRange(tap1);              //Θ(1)  n=4
            bytes2.AddRange(redL);              //Θ(1)  n=4
            bytes2.AddRange(greenL);            //Θ(1)  n=4
            bytes2.AddRange(blueL);             //Θ(1)  n=4

            Byte[] bytes = new byte[tempRed.Count / 8 + (tempRed.Count % 8 == 0 ? 0 : 1)];   //Θ(1)
            BitArray d = new BitArray(tempRed.ToArray());   //Θ(n)  n is size of tempRed
            d.CopyTo(bytes, 0);                     //Θ(N)  n is size of Bytes array
            b.Write(bytes2.ToArray()); //Θ(N^2)  n is size of Bytes array
            b.Write(bytes.ToArray());   //Θ(N)  n is size of Bytes array
            b.Close(); //Θ(1)
        }

        public static RGBPixel[,] decompress(string path)
        {
            int fileoffset;//Θ(1)
            byte[] fileData = FileToByteArray(path);
            int RedTreesize = BitConverter.ToInt32(fileData, 0); //Θ(1)  n=4
            Huffman huffmanRed = new Huffman(getPriorityQueue(Encoding.ASCII.GetString(fileData, 4, RedTreesize))); //Θ(N) n is size of redTree

            int greenTreesize = BitConverter.ToInt32(fileData, RedTreesize + 4);//Θ(1)  n=4
            Huffman huffmanGreen = new Huffman(getPriorityQueue(Encoding.ASCII.GetString(fileData, 8 + RedTreesize, greenTreesize))); //Θ(N) n is size of greenTree

            int blueTreesize = BitConverter.ToInt32(fileData, 8 + greenTreesize + RedTreesize);//Θ(1)  n=4
            Huffman huffmanBlue = new Huffman(getPriorityQueue(Encoding.ASCII.GetString(fileData, 12 + greenTreesize + RedTreesize, blueTreesize))); //Θ(N) n is size of blueTree 
            fileoffset = RedTreesize + blueTreesize + greenTreesize + 12; //Θ(1)

            int width = BitConverter.ToInt32(fileData, fileoffset);//Θ(1)  n=4
            fileoffset += 4;//Θ(1)
            int height = BitConverter.ToInt32(fileData, fileoffset);//Θ(1)  n=4
            fileoffset += 4;//Θ(1)
            int seedLength = BitConverter.ToInt32(fileData, fileoffset);//Θ(1)  n=4
            fileoffset += 4;//Θ(1)

            string seed = Encoding.ASCII.GetString(fileData, fileoffset, seedLength); //Θ(N) n is size of Seed  
            fileoffset += seedLength;//Θ(1)

            int tap = (int)BitConverter.ToInt16(fileData, fileoffset);//Θ(1)  n=2
            fileoffset += 2;//Θ(1)

            int redL = BitConverter.ToInt32(fileData, fileoffset);//Θ(1)  n=4
            fileoffset += 4; //Θ(1)

            int greenL = BitConverter.ToInt32(fileData, fileoffset);//Θ(1)  n=4
            fileoffset += 4;//Θ(1)

            int blueL = BitConverter.ToInt32(fileData, fileoffset);//Θ(1)  n=4
            fileoffset += 4;//Θ(1)

            redL -= greenL;//Θ(1)
            redL -= blueL;//Θ(1)

            byte[] binR = new byte[fileData.Length - fileoffset]; //Θ(1)
            Array.Copy(fileData, fileoffset, binR, 0, fileData.Length - fileoffset); //Θ(N) n is size of BinR

            BitArray bR = new BitArray(binR); //Θ(N) n is size of BinR
            countR = 0;//Θ(1)
            countG = redL;//Θ(1)
            countB = redL + greenL;//Θ(1)

            RGBPixel[,] Image = new RGBPixel[width, height]; //Θ(1)
            //Θ(N^2*K) K is size of binary node represation;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    Image[i, j].red = getColor(huffmanRed.start, bR, ref countR);  //Θ(N) n is size of binary represtation of node

                    Image[i, j].green = getColor(huffmanGreen.start, bR, ref countG); //Θ(N) n is size of binary represtation of node

                    Image[i, j].blue = getColor(huffmanBlue.start, bR, ref countB); //Θ(N) n is size of binary represtation of node


                }
            }

            ImageOperations.encrypt(Image, tap, seed); //Θ(N^2) large between width and height 
            return Image;//Θ(1)
        }
        //Θ(N) n is size of binary represtation of node
        private static byte getColor(Node start, BitArray s, ref int count)
        {
            Node n = start;//Θ(1)

            //Θ(N) n is size of binary represtation of node
            while (n.hasChildreen())
            {
                if (s.Get(count) == false)//Θ(1)
                    n = n.left;//Θ(1)
                else
                    n = n.right;//Θ(1)
                count++; //Θ(1)
            }

            return n.color; //Θ(1)
        }
        //Θ(MLog(n)) n is size of nodes in priorty queue , m length of temp array
        public static PriorityQueue getPriorityQueue(string s)
        {
            PriorityQueue tempDic = new PriorityQueue(); //Θ(1)
            string[] temp = s.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries); //Θ(N) n is size of string
            for (int i = 0; i < temp.Length; i++)
            {
                int color = Int32.Parse(temp[i]); //Θ(1)
                i++; //Θ(1)
                int freq = Int32.Parse(temp[i]); //Θ(1)
                tempDic.Enqueue(new Node((byte)color, freq));
            }
            return tempDic; //Θ(1)

        }
        //Θ(N) n is size of file
        public static byte[] FileToByteArray(string fileName)
        {
            byte[] fileData = null; //Θ(1)

            using (FileStream fs = new FileStream(fileName, FileMode.Open)) //Θ(1)
            {   //Θ(N) n is size of FILE
                using (BinaryReader binaryReader = new BinaryReader(fs))
                {
                    fileData = binaryReader.ReadBytes((int)fs.Length);
                }
            }
            return fileData; //Θ(1)
        }
      
    }
}