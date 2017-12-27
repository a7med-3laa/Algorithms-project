using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageQuantization
{   public class Node
    {
        public Node left, right;            //o(1)
        public int frequnecy;               //o(1)
        public byte color;                  //o(1)
        public List<bool> Binary;           //o(1)
        public Node()
        {
            left = null;                    //o(1)
            right = null;                   //o(1)
            frequnecy = 0;                  //o(1)
            color = 0;                      //o(1)
            Binary = new List<bool>();      //o(1)
        }

        public Node(byte C,int F)
        {
            left = null;                    //o(1)
            right = null;                   //o(1)
            frequnecy = F;                  //o(1)
            color = C;                      //o(1)
            Binary = new List<bool>();      //o(1)
        }
        public bool hasChildreen()
        {
            return !((left == null) && (right == null));    //o(1)
        }
    }


    public class PriorityQueue
    {
        private List<Node> data;                    //o(1)

        public PriorityQueue()
        {
            this.data = new List<Node>(256);        //o(1)
        }
        public PriorityQueue(PriorityQueue temp)
         {
             this.data = new List<Node>();          //o(1)
             for(int i = 0; i<temp.data.Count; i++) //o(n)
             {
                 data.Add(temp.data[i]);            //o(n)
             }
         }
 

       

        public void Enqueue(Node item)
        {
            data.Add(item);                         //o(1)
            int ci = data.Count - 1;                //o(1)
            while (ci > 0)                          //o(log n)
            {
                int pi = (ci - 1) / 2;              //o(log n)
                if (CompareTo(data[ci],data[pi]) >= 0) break;   //o(logn n)
                Node temp = data[ci];               //o(log n)
                data[ci] = data[pi];                //o(log n)
                data[pi] = temp;                    //o(log n)
                ci = pi;                            //o(log n)
            }
        }

        public Node Dequeue()
        {
            int li = data.Count - 1;                //o(1)
            Node frontItem = data[0];               //o(1)
            data[0] = data[li];                     //o(1)
            data.RemoveAt(li);                      //o(1)

            --li;                                   //o(1)
            int pi = 0;                             //o(1)
            while (true)                            //o(log n)
            {
                int ci = pi * 2 + 1;                //o(log n)
                if (ci > li) break;                 //o(log n)
                int rc = ci + 1;                    //o(log n)
                if (rc <= li && CompareTo(data[rc], data[ci]) < 0) //o(log n)
                    ci = rc;                        //o(log n)
                if (CompareTo(data[pi], data[ci]) <= 0) break;   //o(log n)
                Node temp = data[ci];               //o(log n)
                data[ci] = data[pi];                //o(log n)
                data[pi] = temp;                    //o(log n)
                pi = ci;                            //o(log n)
            }
            return frontItem;                       //o(1)
        }

        public Node Peek()
        {
            Node frontItem = data[0];               //o(1)
            return frontItem;                       //o(1)
        }

        public int Count()
        {
            return data.Count;                      //o(1)
        }
        public List<Node> getList()
        {

            return data;
        }
        public int CompareTo(Node other1, Node other2)
        {
            if (other1.frequnecy < other2.frequnecy) return -1;     //o(1)
            else if (other1.frequnecy > other2.frequnecy) return 1; //o(1)
            else return 0;                                          //o(1)
        }
    }
}
