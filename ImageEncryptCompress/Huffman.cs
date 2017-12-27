using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ImageQuantization
{
    class Huffman
    {
        public Node start;                  //o(1)
        PriorityQueue nodes;                //o(1)
        PriorityQueue nodes2;               //o(1)

        public Huffman(PriorityQueue n)
        {
            nodes = n;                      //o(1)
            start = null;                   //o(1)
            nodes2 = new PriorityQueue(n);  //o(1)
            constructTree();                //o(1)

        }


        public List<Node> Colors = new List<Node>();            //o(1)
        public Dictionary<byte, KeyValuePair<int, List<bool>>> ColorsMap = new Dictionary<byte, KeyValuePair<int, List<bool>>>();   //o(1)
        public void getBinary(Node parent, string bin)          //o(n)
        {
            if (parent.right != null)                           //o(n)                
            {
                getBinary(parent.right, bin + "1");             //o(n)
            }
            if (!parent.hasChildreen())                         //o(n)
            {
                List<bool> bb = new List<bool>();               //o(n)
                char[] c = bin.ToCharArray();                   //o(n)
                for (int i = 0; i < c.Length; i++)              //o(n)
                    if (c[i] == '0')                            //o(n)
                        bb.Add(false);                          //o(n)
                    else
                        bb.Add(true);                           //o(n)
                parent.Binary = bb;                             //o(n)
                Colors.Add(parent);                             //o(n)
                ColorsMap.Add(parent.color, new KeyValuePair<int, List<bool>>(parent.frequnecy, parent.Binary));    //o(n)
            }

            if (parent.left != null)                            //o(n)
            {
                getBinary(parent.left, bin + "0");              //o(n)
            }
        }

        private void constructTree()
        {
            Node tmp = new Node();                              //o(log n)
            while (nodes.Count() > 1)                           //o(log n)
            {
                Node right = nodes.Dequeue();                   //o(log n)
                Node left = nodes.Dequeue();                    //o(log n)
                tmp.left = left;                                //o(log n)
                tmp.right = right;
                tmp.frequnecy = left.frequnecy + right.frequnecy;   //o(log n)
                tmp.color = (byte)tmp.frequnecy;                //o(log n)
                nodes.Enqueue(tmp);                             //o(log n)
                tmp = new Node();                               //o(log n)
            }


            start = nodes.Dequeue();                            //o(log n)
            getBinary(start, "");                               //o(log n)
        }
        public String writeHuffman()
        {
            string s = "";                                      //o(n)
            List<Node> dos = nodes2.getList();                  //o(n)
            for (int i = 0; i < dos.Count; i++)                 //o(n)
            {
                Node n = dos[i];                                //o(n)
                s += n.color;                                   //o(n)
                s += ",";                                       //o(n)
                s += n.frequnecy;                               //o(n)
                s += ",";                                       //o(n)

            }
            return s;                                           //o(n)
        }

        public long getCompressedSize()
        {
            long ans = 0;                                       //o(1)
            for (int i = 0; i < Colors.Count; i++)              //o(1)
                ans += ColorsMap[Colors[i].color].Value.Count * ColorsMap[Colors[i].color].Key;           //o(1) 
            return ans;                                         //o(1)
        }
    }
}
