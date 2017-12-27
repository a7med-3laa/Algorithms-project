using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ImageQuantization
{
    class Huffman
    {
        public Node start;                  //ϴ(1)
        PriorityQueue nodes;                //ϴ(1)
        PriorityQueue nodes2;               //ϴ(1)

        public Huffman(PriorityQueue n)
        {
            nodes = n;                      //ϴ(1)
            start = null;                   //ϴ(1)
            nodes2 = new PriorityQueue(n);  //ϴ(1)
            constructTree();                //ϴ(1)

        }


        public List<Node> Colors = new List<Node>();            //ϴ(1)
        public Dictionary<byte, KeyValuePair<int, List<bool>>> ColorsMap = new Dictionary<byte, KeyValuePair<int, List<bool>>>();   //ϴ(1)
        public void getBinary(Node parent, string bin)          //ϴ(K * Z) where k is color count and Z is Binary Size
        {
            if (parent.right != null)                           //ϴ(1)                
            {
                getBinary(parent.right, bin + "1");             //ϴ(1)
            }
            if (!parent.hasChildreen())                         //ϴ(1)
            {
                List<bool> bb = new List<bool>();               //ϴ(1)
                char[] c = bin.ToCharArray();                   //ϴ(1)
                for (int i = 0; i < c.Length; i++)              //ϴ(Z)
                    if (c[i] == '0')                            //ϴ(1)
                        bb.Add(false);                          //ϴ(1)
                    else
                        bb.Add(true);                           //ϴ(1)
                parent.Binary = bb;                             //ϴ(1)
                Colors.Add(parent);                             //ϴ(1)
                ColorsMap.Add(parent.color, new KeyValuePair<int, List<bool>>(parent.frequnecy, parent.Binary));    //ϴ(1)
            }

            if (parent.left != null)                            //ϴ(1)
            {
                getBinary(parent.left, bin + "0");              //ϴ(1)
            }
        }

        private void constructTree()                             //ϴ(n log n)  where n is color size
        {
            Node tmp = new Node();                              //ϴ(1)
            while (nodes.Count() > 1)                           //ϴ(n log n)
            {
                Node right = nodes.Dequeue();                   //ϴ(1)
                Node left = nodes.Dequeue();                    //ϴ(1)
                tmp.left = left;                                //ϴ(1)
                tmp.right = right;                            //ϴ(1)
                tmp.frequnecy = left.frequnecy + right.frequnecy;   //ϴ(1)
                tmp.color = (byte)tmp.frequnecy;                //ϴ(1)
                nodes.Enqueue(tmp);                             //ϴ(log n)
                tmp = new Node();                               //ϴ(1)
            }


            start = nodes.Dequeue();                            //ϴ(1)
            getBinary(start, "");                               //ϴ(1)
        }
        public String writeHuffman()                    //ϴ(K) where K is colour count
        {
            string s = "";                                      //ϴ(1)
            List<Node> dos = nodes2.getList();                  //ϴ(1)
            for (int i = 0; i < dos.Count; i++)                 //ϴ(K)
            {
                Node n = dos[i];                                //ϴ(1)
                s += n.color;                                   //ϴ(1)
                s += ",";                                       //ϴ(1)
                s += n.frequnecy;                               //ϴ(1)
                s += ",";                                       //ϴ(1)

            }
            return s;                                           //ϴ(1)
        }

        public long getCompressedSize()             //ϴ(n) where n is color count
        {
            long ans = 0;                                       //ϴ(1)
            for (int i = 0; i < Colors.Count; i++)              //ϴ(n)
                ans += ColorsMap[Colors[i].color].Value.Count * ColorsMap[Colors[i].color].Key;           //ϴ(1) 
            return ans;                                         //ϴ(1)
        }
    }
}
