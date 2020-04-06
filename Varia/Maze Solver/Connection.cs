using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A_Star_Maze_Solver
{
    class Connection
    {
        private Node my_Node1;
        private Node my_Node2;
        private double my_Distance;

        public Connection(Node n1, Node n2)
        {
            my_Node1 = n1;
            my_Node2 = n2;
            my_Distance = n1.Distance(n2);
        }

        public Connection(Node n1, Node n2, double distance)
        {
            my_Node1 = n1;
            my_Node2 = n2;
            my_Distance = distance;
        }

        public Node Node1
        {
            get { return my_Node1; }
            set { my_Node1 = value; }
        }

        public Node Node2
        {
            get { return my_Node2; }
            set { my_Node2 = value; }
        }

        public double Distance
        {
            get { return my_Distance; }
            set { my_Distance = value; }
        }
    }
}
