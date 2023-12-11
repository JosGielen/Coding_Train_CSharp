using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Marching_Squares
{
    class Marching_Square
    {
        private Vector my_TopLeft;
        private Vector my_TopRight;
        private Vector my_BottomLeft;
        private Vector my_BottomRight;

        /// <summary>
        /// Creates a Marching Square
        /// </summary>
        /// <param name="Location">A Rectangle specifying the Top, Left position and With, Height of the Marching Square.</param>
        public Marching_Square(Rect location)
        {
            my_TopLeft = new Vector(location.Left, location.Top);
            my_TopRight = new Vector(location.Right, location.Top);
            my_BottomLeft = new Vector(location.Left, location.Bottom);
            my_BottomRight = new Vector(location.Right, location.Bottom);
        }

        public Marching_Square(double left, double top, double width, double height)
        {
            my_TopLeft = new Vector(left, top);
            my_TopRight = new Vector(left + width, top);
            my_BottomLeft = new Vector(left, top + height);
            my_BottomRight = new Vector(left + width, top + height);
        }

        public Marching_Square(Vector topLeft, Vector topRight, Vector bottomLeft, Vector bottomRight)
        {
            my_TopLeft = topLeft;
            my_TopRight = topRight;
            my_BottomLeft = bottomLeft;
            my_BottomRight = bottomRight;
        }

        public Marching_Square(Vector topLeft, Vector bottomRight)
        {
            my_TopLeft = new Vector(topLeft.X, topLeft.Y);
            my_TopRight = new Vector(bottomRight.X, topLeft.Y);
            my_BottomLeft = new Vector(topLeft.X, bottomRight.Y);
            my_BottomRight = new Vector(bottomRight.X, bottomRight.Y);
        }

        public Marching_Square(Point topLeft, Point topRight, Point bottomLeft, Point bottomRight)
        {
            my_TopLeft = new Vector(topLeft.X, topLeft.Y);
            my_TopRight = new Vector(topRight.X, topRight.Y);
            my_BottomLeft = new Vector(bottomLeft.X, bottomLeft.Y);
            my_BottomRight = new Vector(bottomRight.X, bottomRight.Y);
        }

        public Marching_Square(Point topLeft, Point bottomRight)
        {
            my_TopLeft = new Vector(topLeft.X, topLeft.Y);
            my_TopRight = new Vector(bottomRight.X, topLeft.Y);
            my_BottomLeft = new Vector(topLeft.X, bottomRight.Y);
            my_BottomRight = new Vector(bottomRight.X, bottomRight.Y);
        }

        /// <summary>
        /// Returns 1 or 2 lines defined by the marching square with the 4 given corner values
        /// </summary>
        /// <param name="CornerValue1">TopLeft Corner Value must be 0 or 1</param>
        /// <param name="cornerValue2">TopRight Corner Value must be 0 or 1</param>
        /// <param name="cornerValue3">BottomLeft Corner Value must be 0 or 1</param>
        /// <param name="cornerValue4">BottomRight Corner Value must be 0 or 1</param>
        /// <returns></returns>
        public List<Line> GetLines(int CornerValue1, int CornerValue2, int CornerValue3, int CornerValue4)
        {
            List<Line> result = new List<Line>();
            Line l;
            int squareType;
            Vector A;
            Vector B;
            squareType = 8 * CornerValue1 + 4 * CornerValue2 + 2 * CornerValue3 + CornerValue4;
            switch (squareType)
            {
                case 0:
                    //Do nothing
                    break;
                case 1:
                    A = 0.5 * (my_TopLeft + my_BottomLeft);
                    B = 0.5 * (my_BottomRight + my_BottomLeft);
                    l = new Line()
                    {
                        X1 = A.X,
                        Y1 = A.Y,
                        X2 = B.X,
                        Y2 = B.Y
                    };
                    result.Add(l);
                    break;
                case 2:
                    A = 0.5 * (my_TopRight + my_BottomRight);
                    B = 0.5 * (my_BottomRight + my_BottomLeft);
                    l = new Line()
                    {
                        X1 = A.X,
                        Y1 = A.Y,
                        X2 = B.X,
                        Y2 = B.Y
                    };
                    result.Add(l);
                    break;
                case 3:
                    A = 0.5 * (my_TopLeft + my_BottomLeft);
                    B = 0.5 * (my_TopRight + my_BottomRight);
                    l = new Line()
                    {
                        X1 = A.X,
                        Y1 = A.Y,
                        X2 = B.X,
                        Y2 = B.Y
                    };
                    result.Add(l);
                    break;
                case 4:
                    A = 0.5 * (my_TopLeft + my_TopRight);
                    B = 0.5 * (my_TopRight + my_BottomRight);
                    l = new Line()
                    {
                        X1 = A.X,
                        Y1 = A.Y,
                        X2 = B.X,
                        Y2 = B.Y
                    };
                    result.Add(l);
                    break;
                case 5:
                    A = 0.5 * (my_TopLeft + my_BottomLeft);
                    B = 0.5 * (my_TopLeft + my_TopRight);
                    l = new Line()
                    {
                        X1 = A.X,
                        Y1 = A.Y,
                        X2 = B.X,
                        Y2 = B.Y
                    };
                    result.Add(l);
                    A = 0.5 * (my_BottomLeft + my_BottomRight);
                    B = 0.5 * (my_TopRight + my_BottomRight);
                    l = new Line()
                    {
                        X1 = A.X,
                        Y1 = A.Y,
                        X2 = B.X,
                        Y2 = B.Y
                    };
                    result.Add(l);
                    break;
                case 6:
                    A = 0.5 * (my_TopLeft + my_TopRight);
                    B = 0.5 * (my_BottomLeft + my_BottomRight);
                    l = new Line()
                    {
                        X1 = A.X,
                        Y1 = A.Y,
                        X2 = B.X,
                        Y2 = B.Y
                    };
                    result.Add(l);
                    break;
                case 7:
                    A = 0.5 * (my_TopLeft + my_TopRight);
                    B = 0.5 * (my_TopLeft + my_BottomLeft);
                    l = new Line()
                    {
                        X1 = A.X,
                        Y1 = A.Y,
                        X2 = B.X,
                        Y2 = B.Y
                    };
                    result.Add(l);
                    break;
                case 8:
                    A = 0.5 * (my_TopLeft + my_TopRight);
                    B = 0.5 * (my_TopLeft + my_BottomLeft);
                    l = new Line()
                    {
                        X1 = A.X,
                        Y1 = A.Y,
                        X2 = B.X,
                        Y2 = B.Y
                    };
                    result.Add(l);
                    break;
                case 9:
                    A = 0.5 * (my_TopLeft + my_TopRight);
                    B = 0.5 * (my_BottomLeft + my_BottomRight);
                    l = new Line()
                    {
                        X1 = A.X,
                        Y1 = A.Y,
                        X2 = B.X,
                        Y2 = B.Y
                    };
                    result.Add(l);
                    break;
                case 10:
                    A = 0.5 * (my_TopLeft + my_TopRight);
                    B = 0.5 * (my_TopRight + my_BottomRight);
                    l = new Line()
                    {
                        X1 = A.X,
                        Y1 = A.Y,
                        X2 = B.X,
                        Y2 = B.Y
                    };
                    result.Add(l);
                    A = 0.5 * (my_TopLeft + my_BottomLeft);
                    B = 0.5 * (my_BottomLeft + my_BottomRight);
                    l = new Line()
                    {
                        X1 = A.X,
                        Y1 = A.Y,
                        X2 = B.X,
                        Y2 = B.Y
                    };
                    result.Add(l);
                    break;
                case 11:
                    A = 0.5 * (my_TopLeft + my_TopRight);
                    B = 0.5 * (my_TopRight + my_BottomRight);
                    l = new Line()
                    {
                        X1 = A.X,
                        Y1 = A.Y,
                        X2 = B.X,
                        Y2 = B.Y
                    };
                    result.Add(l);
                    break;
                case 12:
                    A = 0.5 * (my_TopLeft + my_BottomLeft);
                    B = 0.5 * (my_TopRight + my_BottomRight);
                    l = new Line()
                    {
                        X1 = A.X,
                        Y1 = A.Y,
                        X2 = B.X,
                        Y2 = B.Y
                    };
                    result.Add(l);
                    break;
                case 13:
                    A = 0.5 * (my_TopRight + my_BottomRight);
                    B = 0.5 * (my_BottomRight + my_BottomLeft);
                    l = new Line()
                    {
                        X1 = A.X,
                        Y1 = A.Y,
                        X2 = B.X,
                        Y2 = B.Y
                    };
                    result.Add(l);
                    break;
                case 14:
                    A = 0.5 * (my_TopLeft + my_BottomLeft);
                    B = 0.5 * (my_BottomRight + my_BottomLeft);
                    l = new Line()
                    {
                        X1 = A.X,
                        Y1 = A.Y,
                        X2 = B.X,
                        Y2 = B.Y
                    };
                    result.Add(l);
                    break;
                case 15:
                    //Do nothing
                    break;
            }
            return result;
        }
    }
}

