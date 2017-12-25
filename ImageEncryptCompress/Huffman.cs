using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ImageQuantization
{
    class Huffman
    {
        public Node start;
        PriorityQueue nodes;
        PriorityQueue nodes2;

        public Huffman(PriorityQueue n)
        {
            nodes = n;
            start = null;
            nodes2 = new PriorityQueue(n);
            constructTree();

        }
        

        public List<Node> Colors = new List<Node>();
        public Dictionary<byte, KeyValuePair<int, List<bool>>> ColorsMap = new Dictionary<byte, KeyValuePair<int, List<bool>>>();
        public void getBinary(Node parent,string bin)
        {
            if (parent.right != null)
            {
                getBinary(parent.right, bin + "1");
            }
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

            if (parent.left != null)
            {
                getBinary(parent.left, bin + "0");
            }
        }

        private void constructTree()
        {
            Node tmp = new Node();
            while (nodes.Count() > 1)
            {
                Node right = nodes.Dequeue();
                Node left = nodes.Dequeue();
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
        public String writeHuffman()
        {
            string s = "";
            List<Node> dos = nodes2.getList();
            for (int i = 0; i < dos.Count; i++)
            {
                Node n = dos[i];
                s += n.color;
                s += ",";
                s += n.frequnecy;
                s += ",";

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
