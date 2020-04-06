using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace A_Star_Pathfinder
{
    class Node
    {
        private readonly int my_Row;
        private readonly int my_Col;
        private Point my_Location;
        private double my_F;
        private double my_G;
        private Node my_Previous;

        public Node(int col, int row, double width, double height)
        {
            my_Row = row;
            my_Col = col;
            my_Location = new Point(col * width, row * height);
            my_F = Double.MaxValue;
            my_G = Double.MaxValue;
        }

        public Point Location
        {
            get { return my_Location; }
        }

        public double G
        {
            get { return my_G; }
            set { my_G = value; }
        }

        public double F
        {
            get { return my_F; }
            set { my_F = value; }
        }

        public Node Previous
        {
            get { return my_Previous; }
            set { my_Previous = value; }
        }

        public int Row
        {
            get { return my_Row; }
        }

        public int Col
        {
            get { return my_Col; }
        }

        public double Distance(Node otherNode)
        {
            return Math.Sqrt((my_Location.X - otherNode.Location.X) * (my_Location.X - otherNode.Location.X) + (my_Location.Y - otherNode.Location.Y) * (my_Location.Y - otherNode.Location.Y));
        }
    }
}
