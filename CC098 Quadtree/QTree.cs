using System.Collections.Generic;
using System.Windows;

namespace Quadtree
{
    class QTree
    {
        private Rect my_Boundary;
        private int my_Capacity;
        private List<Point> my_Points;
        private bool divided;
        private QTree[] my_Branches = new QTree[4];

        public QTree(Rect boundary, int capacity)
        {
            my_Boundary = boundary;
            my_Capacity = capacity;
            my_Points = new List<Point>();
            divided = false;
            my_Branches[0] = null;
            my_Branches[1] = null;
            my_Branches[2] = null;
            my_Branches[3] = null;
        }

        #region "Properties"

        public Rect Boundary
        {
            get { return my_Boundary; }
        }

        public int Capacity
        {
            get { return my_Capacity; }
        }

        public List<Point> Points
        {
            get { return my_Points; }
        }

        public bool IsDivided
        {
            get { return divided; }
        }

        public QTree[] Branches
        {
            get { return my_Branches; }
        }

        #endregion

        public bool Insert(Point pt)
        {
            bool result;
            if (pt.X < my_Boundary.Left || pt.X > my_Boundary.Right || pt.Y < my_Boundary.Top || pt.Y > my_Boundary.Bottom)
            {
                return false;
            }
            if (my_Points.Count < my_Capacity)
            {
                my_Points.Add(pt);
                result = true;
            }
            else
            {
                if (!divided)
                {
                    my_Branches[0] = CreateSubTree(0);
                    my_Branches[1] = CreateSubTree(1);
                    my_Branches[2] = CreateSubTree(2);
                    my_Branches[3] = CreateSubTree(3);
                    divided = true;
                }
                result = my_Branches[0].Insert(pt);
                if (!result) result = my_Branches[1].Insert(pt);
                if (!result) result = my_Branches[2].Insert(pt);
                if (!result) result = my_Branches[3].Insert(pt);
            }
            return result;
        }

        public List<Point> Query(Rect area)
        {
            List<Point> result = new List<Point>();
            if (!my_Boundary.IntersectsWith(area))
            {
                return result;
            }
            else
            {
                foreach (Point pt in my_Points)
                {
                    if (area.Contains(pt))
                    {
                        result.Add(pt);
                    }
                }
                if (divided)
                {
                    result.AddRange(my_Branches[0].Query(area));
                    result.AddRange(my_Branches[1].Query(area));
                    result.AddRange(my_Branches[2].Query(area));
                    result.AddRange(my_Branches[3].Query(area));
                }
            }
            return result;
        }

        public List<Point> Query(Point center, double distance)
        {
            List<Point> result = new List<Point>();
            if (!my_Boundary.IntersectsWith(new Rect(center.X - distance, center.Y - distance, 2 * distance, 2 * distance)))
            {
                return result;
            }
            else
            {
                foreach (Point pt in my_Points)
                {
                    if ((pt.X - center.X) * (pt.X - center.X) + (pt.Y - center.Y) * (pt.Y - center.Y) < distance * distance)
                    {
                        result.Add(pt);
                    }
                }
                if (divided)
                {
                    result.AddRange(my_Branches[0].Query(center, distance));
                    result.AddRange(my_Branches[1].Query(center, distance));
                    result.AddRange(my_Branches[2].Query(center, distance));
                    result.AddRange(my_Branches[3].Query(center, distance));
                }
            }
            return result;
        }

        public void Clear()
        {
            my_Points.Clear();
            divided = false;
            my_Branches[0] = null;
            my_Branches[1] = null;
            my_Branches[2] = null;
            my_Branches[3] = null;
        }

        private QTree CreateSubTree(int nr)
        {
            QTree result;
            double top = 0;
            double left = 0;
            double width = my_Boundary.Width / 2;
            double height = my_Boundary.Height / 2;
            switch (nr)
            {
                case 0: //Top Left
                {
                    top = my_Boundary.Top;
                    left = my_Boundary.Left;
                    break;
                }
                case 1: //Top Right
                {
                    top = my_Boundary.Top;
                    left = my_Boundary.Left + width;
                    break;
                }
                case 2: //Bottom Left
                {
                    top = my_Boundary.Top + height;
                    left = my_Boundary.Left;
                    break;
                }
                case 3: //Bottom Right
                {
                    top = my_Boundary.Top + height;
                    left = my_Boundary.Left + width;
                    break;
                }
                default:
                {
                    break;
                }
            }
            result = new QTree(new Rect(left, top, width, height), my_Capacity);
            return result;
        }
    }
}
