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
        public Dictionary<byte, KeyValuePair<int, List<bool>>> ColorsMap = new Dictionary<byte, KeyValuePair<int, List<bool>>>();
        public void getBinary(Node parent,string bin)
        {
            if (!parent.hasChildreen())
            {
                List<bool> bb = new List<bool>();
                char[] c = bin.ToCharArray();
                for (int i = 0; i < c.Length; i++)
                    if (c[i] == '0')
                        bb.Add(false);
                    else
                        bb.Add(true);
                parent.Binary = bb;
                Colors.Add(parent);
                ColorsMap.Add(parent.color, new KeyValuePair<int, List<bool>>(parent.frequnecy, parent.Binary));
            }
            if(parent.left != null)
            {
                getBinary(parent.left, bin + "0");
            }
            if (parent.right != null)
            {
                getBinary(parent.right, bin + "1");
            }
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
            //    string filename = "huffman.txt";
            //    File.WriteAllText(filename, "");
        }
        public String writeHuffman()
        {
            string s = "";
            for (int i = 0; i < Colors.Count; i++)
            {
                s += Colors[i].color;
                s += ":";
             
                s += Colors[i].frequnecy;
                s += ":";
                s += Colors[i].Binary;

               
                for(int k = 0; k< jj; k++)
                {
                    if (Colors[i].Binary[k])
                        s += "1";
                    else
                        s += "0";
                }
             
            }
            return s;
        }

        public long getCompressedSize()
        {
            long ans = 0;
            for(int i = 0; i < Colors.Count; i++)
                ans += ColorsMap[Colors[i].color].Value.Count * ColorsMap[Colors[i].color].Key;            
            return ans;
        }
    }
}
