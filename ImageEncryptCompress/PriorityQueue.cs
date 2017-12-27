using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageQuantization
{   public class Node
    {
        public Node left, right;            //ϴ(1)
        public int frequnecy;               //ϴ(1)
        public byte color;                  //ϴ(1)
        public List<bool> Binary;           //ϴ(1)
        public Node()
        {
            left = null;                    //ϴ(1)
            right = null;                   //ϴ(1)
            frequnecy = 0;                  //ϴ(1)
            color = 0;                      //ϴ(1)
            Binary = new List<bool>();      //ϴ(1)
        }

        public Node(byte C,int F)
        {
            left = null;                    //ϴ(1)
            right = null;                   //ϴ(1)
            frequnecy = F;                  //ϴ(1)
            color = C;                      //ϴ(1)
            Binary = new List<bool>();      //ϴ(1)
        }
        public bool hasChildreen()
        {
            return !((left == null) && (right == null));    //ϴ(1)
        }
    }


    public class PriorityQueue
    {
        private List<Node> data;                    //ϴ(1)

        public PriorityQueue()
        {
            this.data = new List<Node>(256);        //ϴ(1)
        }
        public PriorityQueue(PriorityQueue temp)  // ϴ(n) where n is color count
        {
             this.data = new List<Node>();          //ϴ(1)
            for (int i = 0; i<temp.data.Count; i++) //ϴ(n)
            {
                 data.Add(temp.data[i]);            //ϴ(1)
            }
         }
 

       

        public void Enqueue(Node item)      // ϴ(log n) where n is colors size
        {
            data.Add(item);                         //ϴ(1)
            int ci = data.Count - 1;                //ϴ(1)
            while (ci > 0)                          //ϴ(log n)
            {
                int pi = (ci - 1) / 2;              //ϴ(1)
                if (CompareTo(data[ci],data[pi]) >= 0) break;   //ϴ(1)
                Node temp = data[ci];               //ϴ(1)
                data[ci] = data[pi];                //ϴ(1)
                data[pi] = temp;                    //ϴ(1)
                ci = pi;                            //ϴ (1)
            }
        }

        public Node Dequeue()           //ϴ(log n) where n is colour count
        {
            int li = data.Count - 1;                //ϴ(1)
            Node frontItem = data[0];               //ϴ(1)
            data[0] = data[li];                     //ϴ(1)
            data.RemoveAt(li);                      //ϴ(1)

            --li;                                   //ϴ(1)
            int pi = 0;                             //ϴ(1)
            while (true)                            //ϴ(log n)
            {
                int ci = pi * 2 + 1;                //ϴ(1)
                if (ci > li) break;                 //ϴ(1)
                int rc = ci + 1;                    //ϴ(1)
                if (rc <= li && CompareTo(data[rc], data[ci]) < 0) //ϴ(1)
                    ci = rc;                        //ϴ(1)
                if (CompareTo(data[pi], data[ci]) <= 0) break;   //ϴ(1)
                Node temp = data[ci];               //ϴ(1)
                data[ci] = data[pi];                //ϴ(1)
                data[pi] = temp;                    //ϴ(1)
                pi = ci;                            //ϴ(1)
            }
            return frontItem;                       //ϴ(1)
        }
        
        public int Count()                              //ϴ(1)
        {
            return data.Count;                      //ϴ(1)
        }
        public List<Node> getList()                 // ϴ(1)
        {

            return data;                            //ϴ(1)
        }
        public int CompareTo(Node other1, Node other2)                      //ϴ(1)
        {
            if (other1.frequnecy < other2.frequnecy) return -1;             //ϴ(1)
            else if (other1.frequnecy > other2.frequnecy) return 1;       //ϴ(1)
            else return 0;                                          //ϴ(1)
        }
    }
}
