using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ImageQuantization
{
    class Huffman
    {
        Node start;
        PriorityQueue nodes;
        public Huffman(PriorityQueue n)
        {
            nodes = n;
            start = null;
            constructTree();
        }
        

        public List<Node> Colors = new List<Node>();
        public Dictionary<byte, KeyValuePair<int, string>> ColorsMap = new Dictionary<byte, KeyValuePair<int, string>>();
        public void getBinary(Node parent,string bin)
        {
            if (!parent.hasChildreen())
            {
                parent.Binary = bin;
                Colors.Add(parent);
                ColorsMap.Add(parent.color, new KeyValuePair<int, string>(parent.frequnecy, parent.Binary));
            }
            if(parent.left != null)
                getBinary(parent.left, bin + "0");
            if (parent.right != null)
                getBinary(parent.right, bin + "1");
        }

        private void constructTree()
        {
            Node tmp = new Node();
            while (nodes.Count() > 1)
            {
                Node right = nodes.Dequeue();
                Node left = nodes.Dequeue();

                if(left.color > right.color && (!left.hasChildreen() && !right.hasChildreen()))
                {
                    Node temp = right;
                    right = left;
                    left = temp;
                }

                if(left.hasChildreen() && !right.hasChildreen())
                {
                    Node temp = right;
                    right = left;
                    left = temp;
                }
                tmp.left = left;
                tmp.right = right;
                tmp.frequnecy = left.frequnecy + right.frequnecy;
                tmp.color = (byte) tmp.frequnecy;
                nodes.Enqueue(tmp);
                tmp = new Node();
            }


            start = nodes.Dequeue();
            getBinary(start,"");
        }

        static public void clearFile()
        {
            string filename = "huffman.txt";
            string s = " Color  -  Frequency  -  Binary  -  Total Bits  " + Environment.NewLine;
            File.WriteAllText(filename, s);
        }
        public void writeHuffman(string color)
        {
            string filename = "huffman.txt";
            if (!File.Exists(filename))
                File.Create(filename);
            string s = "--------- " + color + " ---------" + Environment.NewLine;
            for (int i = 0; i < Colors.Count; i++)
            {
                s += Colors[i].color;
                s += "  -  ";
             
                s += Colors[i].frequnecy;
                s += "  -  ";
                s += Colors[i].Binary;
                s += "  -  ";
                long total = Colors[i].frequnecy * Colors[i].Binary.Length;
                s += total.ToString();
                s += Environment.NewLine;
            }
            s += Environment.NewLine;
            File.AppendAllText(filename, s);
        }

        public long getCompressedSize()
        {
            long ans = 0;
            for(int i = 0; i < Colors.Count; i++)
                ans += ColorsMap[Colors[i].color].Value.Length * ColorsMap[Colors[i].color].Key;            
            return ans;
        }
    }
}
