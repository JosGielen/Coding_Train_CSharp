using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Star_Patterns
{
    internal class Tile
    {
        private Polygon my_Poly;
        private Brush my_LineColor;
        private double my_LineThickness;
        private Brush my_FillColor;
        private Brush my_StarColor;
        private List<Hankin> HankinLines;
        private PointCollection my_StarPoints;
        private Polygon my_Star;
        private List<Line> my_Lines;

        public Tile()
        {
            my_Poly = new Polygon();
            my_LineColor = Brushes.Black;
            my_FillColor = Brushes.White;
            my_StarColor = Brushes.Transparent;
            my_LineThickness = 1.0;
        }

        public PointCollection Points
        {
            get { return my_Poly.Points; }
            set { my_Poly.Points = value; }
        }

        public Brush LineColor
        {
            get { return my_LineColor; }
            set { my_LineColor = value; }
        }

        public double LineThickness
        {
            get { return my_LineThickness; }
            set { my_LineThickness = value; }
        }

        public Brush FillColor
        {
            get { return my_FillColor; }
            set { my_FillColor = value; }
        }

        public Brush StarColor
        {
            get { return my_StarColor; }
            set { my_StarColor = value; }
        }

        public void AddPoint(double X, double Y)
        {
            my_Poly.Points.Add(new Point(X, Y));
}

        public void MakeStar(double delta, double angle)
        {
            my_Poly.Points.Add(my_Poly.Points[0]);
            Matrix mrot;
            Point bestPt;
            HankinLines = new List<Hankin>();
            my_Lines = new List<Line>();
            my_StarPoints = new PointCollection();
            //Make the Hankins
            Hankin h1;
            Hankin h2;
            Vector V;
            for (int I = 0; I < my_Poly.Points.Count - 1; I++)
            {
                h1 = new Hankin();
                h2 = new Hankin();
                V = my_Poly.Points[I + 1] - my_Poly.Points[I];
                h1.pt = my_Poly.Points[I] + (0.5 + delta / 100) * V;
                mrot = new Matrix();
                mrot.Rotate(angle);
                h1.dir = mrot.Transform(V);
                HankinLines.Add(h1);
                h2.pt = my_Poly.Points[I] + (0.5 - delta / 100) * V;
                mrot = new Matrix();
                mrot.Rotate(180 - angle);
                h2.dir = mrot.Transform(V);
                HankinLines.Add(h2);
            }

            //Find the closest intersects of the Hankins
            Point intPt;
            double dist;
            double mindist;
            int closestindex;
            Line l;
            for (int I = 0; I < HankinLines.Count; I++)
            {
                mindist = double.MaxValue;
                closestindex = -1;
                for (int J = 0; J < HankinLines.Count; J++)
                {
                    if (I != J)
                    {
                        intPt = Intersect(HankinLines[I], HankinLines[J]);
                        if (intPt.X > -10000 & intPt.Y > -10000)
                        {
                            dist = Distsquared(HankinLines[I].pt, intPt) + Distsquared(intPt, HankinLines[J].pt);
                            if (dist < mindist)
                            {
                                mindist = dist;
                                closestindex = J;
                                bestPt = intPt;
                            }
                        }
                    }
                }
                if (closestindex >= 0)
                {
                    my_StarPoints.Add(HankinLines[I].pt);
                    my_StarPoints.Add(bestPt);
                    my_StarPoints.Add(HankinLines[closestindex].pt);
                    l = new Line()
                    {
                        X1 = HankinLines[I].pt.X,
                        Y1 = HankinLines[I].pt.Y,
                        X2 = bestPt.X,
                        Y2 = bestPt.Y
                    };
                    my_Lines.Add(l);
                    l = new Line()
                    {
                        X1 = bestPt.X,
                        Y1 = bestPt.Y,
                        X2 = HankinLines[closestindex].pt.X,
                        Y2 = HankinLines[closestindex].pt.Y
                    };
                    my_Lines.Add(l);
                }
            }
            MakeStarPolygon();
        }

        public double Distsquared(Point pt1, Point pt2)
        {
            return (pt1.X - pt2.X) * (pt1.X - pt2.X) + (pt1.Y - pt2.Y) * (pt1.Y - pt2.Y);
        }

        public Point Intersect(Hankin H1, Hankin H2)
        {
            Point result = new Point();
            double nom = H2.dir.Y * H1.dir.X - H2.dir.X * H1.dir.Y;
            double UA = (H2.dir.X * (H1.pt.Y - H2.pt.Y) - H2.dir.Y * (H1.pt.X - H2.pt.X)) / nom;
            double UB = (H1.dir.X * (H1.pt.Y - H2.pt.Y) - H1.dir.Y * (H1.pt.X - H2.pt.X)) / nom;
            if (UA > 0 & UB > 0)
            {
                result.X = H1.pt.X + UA * H1.dir.X;
                result.Y = H1.pt.Y + UA * H1.dir.Y;
            }
            else
            {
                result.X = -10000;
                result.Y = -10000;
            }
            return result;
        }

        private void MakeStarPolygon()
        {
            //Eliminate double points
            PointCollection pts = new PointCollection();
            bool isOK;
            for (int I = 0; I < my_StarPoints.Count; I++)
            {
                isOK = true;
                for (int J = 0; J < pts.Count; J++)
                {
                    if (Distsquared(my_StarPoints[I], pts[J]) < 1)
                    {
                        isOK = false;
                        break;
                    }
                }
                if (isOK) pts.Add(my_StarPoints[I]);
            }
            my_StarPoints = pts;
            //Find the center of the Star
            double centerX = 0.0;
            double centerY = 0.0;
            Point center;
            List<double> angles = new List<double>();
            for (int I = 0; I < my_StarPoints.Count; I++)
            {
                centerX += my_StarPoints[I].X;
                centerY += my_StarPoints[I].Y;
            }
            center = new Point(centerX / my_StarPoints.Count, centerY / my_StarPoints.Count);
            //Get the angle from each point towards the center
            Vector V;
            for (int I = 0; I < my_StarPoints.Count; I++)
            {
                V = my_StarPoints[I] - center;
                angles.Add(Vector.AngleBetween(V, new Vector(1, 0)));
            }
            //Sort the angles and the corresponding points in increasing order
            double dummyAngle;
            Point dummyPoint;
            for (int I = 0; I < my_StarPoints.Count; I++)
            {
                for (int J = I + 1; J < my_StarPoints.Count; J++)
                {
                    if (angles[J] < angles[I])
                    {
                        dummyAngle = angles[I];
                        angles[I] = angles[J];
                        angles[J] = dummyAngle;
                        dummyPoint = my_StarPoints[I];
                        my_StarPoints[I] = my_StarPoints[J];
                        my_StarPoints[J] = dummyPoint;
                    }
                }
            }
            my_Star = new Polygon
            {
                Points = my_StarPoints
            };
        }

        public void Draw(Canvas c)
        {
            //Draw the polygon
            my_Poly.Stroke = my_LineColor;
            my_Poly.StrokeThickness = my_LineThickness;
            my_Poly.Fill = my_FillColor;
            c.Children.Add(my_Poly);
        }

        public void DrawStar(Canvas c)
        {
            //Draw the star
            if (my_Star != null ) 
            {
                my_Star.Stroke = my_LineColor;
                my_Star.StrokeThickness = my_LineThickness;
                my_Star.Fill = my_StarColor;
                c.Children.Add(my_Star);
            }
        }

        public void DrawLines(Canvas c)
        {
            //Draw the Hankin Lines
            for (int I = 0; I < my_Lines.Count; I++)
            {
                my_Lines[I].Stroke = my_StarColor;
                my_Lines[I].StrokeThickness = my_LineThickness;
                c.Children.Add(my_Lines[I]);
            }
        }
    }

    public class Hankin
    {
        public Point pt;
        public Vector dir;
    }
}
