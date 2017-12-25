using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageQuantization
{
    public class Node
    {
        public Node left, right;
        public int frequnecy;
        public byte color;
        public List<bool> Binary;
        public Node()
        {
            left = null;
            right = null;
            frequnecy = 0;
            color = 0;
            Binary = new List<bool>();
        }

        public Node(byte C,int F)
        {
            left = null;
            right = null;
            frequnecy = F;
            color = C;
            Binary = new List<bool>();
        }
        public bool hasChildreen()
        {
            return !((left == null) && (right == null));
        }
    }


    public class PriorityQueue
    {
        private List<Node> data;

        public PriorityQueue()
        {
            this.data = new List<Node>(256);
        }

        public PriorityQueue(PriorityQueue temp)
        {
            this.data = new List<Node>();
            for(int i = 0; i < temp.data.Count; i++)
            {
                data.Add(temp.data[i]);
            }
        }

        public void Enqueue(Node item)
        {
            data.Add(item);
            int ci = data.Count - 1; 
            while (ci > 0)
            {
                int pi = (ci - 1) / 2; 
                if (CompareTo(data[ci],data[pi]) >= 0) break; 
                Node temp = data[ci];
                data[ci] = data[pi];
                data[pi] = temp;
                ci = pi;
            }
        }

        public Node Dequeue()
        {
            int li = data.Count - 1;
            Node frontItem = data[0];   
            data[0] = data[li];
            data.RemoveAt(li);

            --li; 
            int pi = 0;
            while (true)
            {
                int ci = pi * 2 + 1; 
                if (ci > li) break;  
                int rc = ci + 1;     
                if (rc <= li && CompareTo(data[rc],data[ci])< 0) 
                    ci = rc;
                if (CompareTo(data[pi],data[ci]) <= 0) break;
                Node temp = data[ci];
                data[ci] = data[pi];
                data[pi] = temp;
                pi = ci;
            }
            return frontItem;
        }

        public Node Peek()
        {
            Node frontItem = data[0];
            return frontItem;
        }

        public int Count()
        {
            return data.Count;
        }
        public int CompareTo(Node other1, Node other2)
        {
            if (other1.frequnecy < other2.frequnecy) return -1;
            else if (other1.frequnecy > other2.frequnecy) return 1;
            else return 0;
        }
    }
}
