using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Dragon_Curve
{
    internal class LineSequence
    {
        public Polyline pLine {  get; set; }
        public Point Pivot {  get; set; }
        public LineSequence() 
        {
            pLine = new Polyline()
            {
                Stroke = Brushes.Black,
                StrokeThickness = 20,
                SnapsToDevicePixels = true
            };
        }

        public void AddPoint(Point newPt)
        {
            pLine.Points.Add(new Point(newPt.X, newPt.Y));
        }

       public LineSequence Copy()
        {
            LineSequence result = new LineSequence();
            for (int I = 0; I < pLine.Points.Count; I++)
            {
                result.pLine.Points.Add(pLine.Points[I]);
            }
            result.pLine.Stroke = pLine.Stroke;
            result.pLine.StrokeThickness = pLine.StrokeThickness;
            return result;
        }

        public void Draw(Canvas c)
        {
            c.Children.Add(pLine);
        }

        public void Rotate(double angle)
        {
            double dX, dY;
            double newX, newY;
            for (int I = 0; I < pLine.Points.Count; I++)
            {
                dX = pLine.Points[I].X - Pivot.X;
                dY = pLine.Points[I].Y - Pivot.Y;
                newX = Pivot.X + dX * Math.Cos(angle) - dY * Math.Sin(angle);
                newY = Pivot.Y + dX * Math.Sin(angle) + dY * Math.Cos(angle);
                pLine.Points[I] = new Point(newX, newY);
            }
        }
    }
}
