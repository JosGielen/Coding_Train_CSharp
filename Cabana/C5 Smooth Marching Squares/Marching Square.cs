using System.Collections.Generic;
using System.Windows;
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
        /// <param name="TopLeftValue">TopLeft Corner Value must be 0 or 1</param>
        /// <param name="cornerValue2">TopRight Corner Value must be 0 or 1</param>
        /// <param name="cornerValue3">BottomLeft Corner Value must be 0 or 1</param>
        /// <param name="cornerValue4">BottomRight Corner Value must be 0 or 1</param>
        /// <returns></returns>
        public List<Line> GetLines(int TopLeftValue, int TopRightValue, int BottomLeftValue, int BottomRightValue)
        {
            List<Line> result = new List<Line>();
            Line l;
            int squareType;
            Vector A = 0.5 * (my_TopLeft + my_TopRight);
            Vector B = 0.5 * (my_TopRight + my_BottomRight);
            Vector C = 0.5 * (my_BottomLeft + my_BottomRight);
            Vector D = 0.5 * (my_TopLeft + my_BottomLeft);
            squareType = 8 * TopLeftValue + 4 * TopRightValue + 2 * BottomRightValue + BottomLeftValue;
            switch (squareType)
            {
                case 0:
                    //Do nothing
                    break;
                case 1:
                    l = new Line()
                    {
                        X1 = C.X,
                        Y1 = C.Y,
                        X2 = D.X,
                        Y2 = D.Y
                    };
                    result.Add(l);
                    break;
                case 2:
                    l = new Line()
                    {
                        X1 = B.X,
                        Y1 = B.Y,
                        X2 = C.X,
                        Y2 = C.Y
                    };
                    result.Add(l);
                    break;
                case 3:
                    l = new Line()
                    {
                        X1 = B.X,
                        Y1 = B.Y,
                        X2 = D.X,
                        Y2 = D.Y
                    };
                    result.Add(l);
                    break;
                case 4:
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
                    l = new Line()
                    {
                        X1 = A.X,
                        Y1 = A.Y,
                        X2 = D.X,
                        Y2 = D.Y
                    };
                    result.Add(l);
                    l = new Line()
                    {
                        X1 = B.X,
                        Y1 = B.Y,
                        X2 = C.X,
                        Y2 = C.Y
                    };
                    result.Add(l);
                    break;
                case 6:
                    l = new Line()
                    {
                        X1 = A.X,
                        Y1 = A.Y,
                        X2 = C.X,
                        Y2 = C.Y
                    };
                    result.Add(l);
                    break;
                case 7:
                    l = new Line()
                    {
                        X1 = A.X,
                        Y1 = A.Y,
                        X2 = D.X,
                        Y2 = D.Y
                    };
                    result.Add(l);
                    break;
                case 8:
                    l = new Line()
                    {
                        X1 = A.X,
                        Y1 = A.Y,
                        X2 = D.X,
                        Y2 = D.Y
                    };
                    result.Add(l);
                    break;
                case 9:
                    l = new Line()
                    {
                        X1 = A.X,
                        Y1 = A.Y,
                        X2 = C.X,
                        Y2 = C.Y
                    };
                    result.Add(l);
                    break;
                case 10:
                    l = new Line()
                    {
                        X1 = A.X,
                        Y1 = A.Y,
                        X2 = B.X,
                        Y2 = B.Y
                    };
                    result.Add(l);
                    l = new Line()
                    {
                        X1 = C.X,
                        Y1 = C.Y,
                        X2 = D.X,
                        Y2 = D.Y
                    };
                    result.Add(l);
                    break;
                case 11:
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
                    l = new Line()
                    {
                        X1 = B.X,
                        Y1 = B.Y,
                        X2 = D.X,
                        Y2 = D.Y
                    };
                    result.Add(l);
                    break;
                case 13:
                    l = new Line()
                    {
                        X1 = B.X,
                        Y1 = B.Y,
                        X2 = C.X,
                        Y2 = C.Y
                    };
                    result.Add(l);
                    break;
                case 14:
                    l = new Line()
                    {
                        X1 = C.X,
                        Y1 = C.Y,
                        X2 = D.X,
                        Y2 = D.Y
                    };
                    result.Add(l);
                    break;
                case 15:
                    //Do nothing
                    break;
            }
            return result;
        }

        /// <summary>
        /// Returns 1 or 2 lines defined by the marching square with the 4 given corner values
        /// </summary>
        /// <param name="TopLeftValue">TopLeft Corner Value must be between 0 and 1</param>
        /// <param name="cornerValue2">TopRight Corner Value must be between 0 and 1</param>
        /// <param name="cornerValue3">BottomLeft Corner Value must be between 0 and 1</param>
        /// <param name="cornerValue4">BottomRight Corner Value must be between 0 and 1</param>
        /// <returns></returns>
        public List<Line> GetSmoothLines(double TopLeftValue, double TopRightValue, double BottomLeftValue, double BottomRightValue)
        {
            List<Line> result = new List<Line>();
            Line l;
            int TopLeft = TopLeftValue < 0.5 ? 0 : 1;
            int TopRight = TopRightValue < 0.5 ? 0 : 1;
            int BottomLeft = BottomLeftValue < 0.5 ? 0 : 1;
            int BottomRight = BottomRightValue < 0.5 ? 0 : 1;
            Vector A = LerpX(my_TopLeft, my_TopRight, TopLeftValue, TopRightValue);
            Vector B = LerpY(my_TopRight, my_BottomRight, TopRightValue, BottomRightValue);
            Vector C = LerpX(my_BottomLeft, my_BottomRight, BottomLeftValue, BottomRightValue);
            Vector D = LerpY(my_TopLeft, my_BottomLeft, TopLeftValue, BottomLeftValue);
            int squareType = 8 * TopLeft + 4 * TopRight + 2 * BottomRight + BottomLeft;
            switch (squareType)
            {
                case 0:
                    //Do nothing
                    break;
                case 1:
                    l = new Line()
                    {
                        X1 = C.X,
                        Y1 = C.Y,
                        X2 = D.X,
                        Y2 = D.Y
                    };
                    result.Add(l);
                    break;
                case 2:
                    l = new Line()
                    {
                        X1 = B.X,
                        Y1 = B.Y,
                        X2 = C.X,
                        Y2 = C.Y
                    };
                    result.Add(l);
                    break;
                case 3:
                    l = new Line()
                    {
                        X1 = B.X,
                        Y1 = B.Y,
                        X2 = D.X,
                        Y2 = D.Y
                    };
                    result.Add(l);
                    break;
                case 4:
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
                    l = new Line()
                    {
                        X1 = A.X,
                        Y1 = A.Y,
                        X2 = D.X,
                        Y2 = D.Y
                    };
                    result.Add(l);
                    l = new Line()
                    {
                        X1 = B.X,
                        Y1 = B.Y,
                        X2 = C.X,
                        Y2 = C.Y
                    };
                    result.Add(l);
                    break;
                case 6:
                    l = new Line()
                    {
                        X1 = A.X,
                        Y1 = A.Y,
                        X2 = C.X,
                        Y2 = C.Y
                    };
                    result.Add(l);
                    break;
                case 7:
                    l = new Line()
                    {
                        X1 = A.X,
                        Y1 = A.Y,
                        X2 = D.X,
                        Y2 = D.Y
                    };
                    result.Add(l);
                    break;
                case 8:
                    l = new Line()
                    {
                        X1 = A.X,
                        Y1 = A.Y,
                        X2 = D.X,
                        Y2 = D.Y
                    };
                    result.Add(l);
                    break;
                case 9:
                    l = new Line()
                    {
                        X1 = A.X,
                        Y1 = A.Y,
                        X2 = C.X,
                        Y2 = C.Y
                    };
                    result.Add(l);
                    break;
                case 10:
                    l = new Line()
                    {
                        X1 = A.X,
                        Y1 = A.Y,
                        X2 = B.X,
                        Y2 = B.Y
                    };
                    result.Add(l);
                    l = new Line()
                    {
                        X1 = C.X,
                        Y1 = C.Y,
                        X2 = D.X,
                        Y2 = D.Y
                    };
                    result.Add(l);
                    break;
                case 11:
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
                    l = new Line()
                    {
                        X1 = B.X,
                        Y1 = B.Y,
                        X2 = D.X,
                        Y2 = D.Y
                    };
                    result.Add(l);
                    break;
                case 13:
                    l = new Line()
                    {
                        X1 = B.X,
                        Y1 = B.Y,
                        X2 = C.X,
                        Y2 = C.Y
                    };
                    result.Add(l);
                    break;
                case 14:
                    l = new Line()
                    {
                        X1 = C.X,
                        Y1 = C.Y,
                        X2 = D.X,
                        Y2 = D.Y
                    };
                    result.Add(l);
                    break;
                case 15:
                    //Do nothing
                    break;
            }
            return result;
        }

        private Vector LerpX (Vector a, Vector b, double aValue, double bValue )
        {
            Vector result = new Vector();
            if (b.X == a.X)
            {
                result.X = 0.5 * (a.X + b.X);
            }
            else
            {
                result.X = a.X + (0.5 - aValue) / (bValue - aValue) * (b.X - a.X);
            }
            result.Y = a.Y;
            return result;
        }

        private Vector LerpY (Vector a, Vector b, double aValue, double bValue)
        {
            Vector result = new Vector();
            if (b.X == a.X)
            {
                result.Y = 0.5 * (a.Y + b.Y);
            }
            else
            {
                result.Y = a.Y + (0.5 - aValue) / (bValue - aValue) * (b.Y - a.Y);
            }

            result.X = a.X;
            return result;
        }
    }
}

