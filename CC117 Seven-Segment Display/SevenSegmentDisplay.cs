using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace _7SegmentDisplay
{
    class SevenSegmentDisplay
    {
        private Rectangle Back;
        private Polygon[] Segments;
        private Ellipse Dot;
        private int my_Value;
        private string[] HexCodes;
        private double my_Width;
        private double my_Height;
        private double my_Left;
        private double my_Top;
        private Brush my_SegmentColor;
        private Brush my_Backcolor;
        private double my_Border;             //% of segment Height
        private double my_SegmentThickness;   //% of segment Height
        private double my_VertSegmentLength;
        private double my_HorSegmentLength;
        private double my_Campher;
        private bool my_HasCampher;
        private double my_SegmentSpace;       //% of segment Height
        private bool my_IsTilted;
        private bool my_ShowDot;

        public SevenSegmentDisplay(double width, double height, double left, double top)
        {
            my_Width = width;
            my_Height = height;
            my_Left = left;
            my_Top = top;
            my_SegmentColor = Brushes.Red;
            my_Backcolor = Brushes.Black;
            my_Border = (int)(3 * my_Height / 100);
            my_SegmentThickness = (int)(10 * my_Height / 100);
            my_HasCampher = true;
            my_Campher = (int)(my_SegmentThickness / 2);
            my_SegmentSpace = (int)(my_Height / 100);
            my_IsTilted = true;
            HexCodes = new string[] { "1111110", "0110000", "1101101", "1111001", "0110011", "1011011", "1011111", "1110000", "1111111", "1111011" };
            Back = new Rectangle();
            Segments = new Polygon[7];
            for (int I = 0; I < 7; I++)
            {
                Segments[I] = new Polygon()
                {
                    Visibility = Visibility.Hidden
                };
            }

            Dot = new Ellipse()
            {
                Visibility = Visibility.Hidden
            };
            MakeSegments();
        }

        #region "Properties"

        public double Width
        {
            get { return my_Width; }
            set
            {
                my_Width = value;
                MakeSegments();
            }
        }

        public double Height
        {
            get { return my_Height ; }
            set
            {
                my_Height = value;
                MakeSegments();
            }
        }

        public double Left
        {
            get { return my_Left; }
            set
            {
                my_Left = value;
                MakeSegments();
            }
        }

        public double Top
        {
            get { return my_Top; }
            set
            {
                my_Top = value;
                MakeSegments();
            }
        }

        public Brush SegmentColor
        {
            get { return my_SegmentColor; }
            set
            {
                my_SegmentColor = value;
                MakeSegments();
            }
        }

        public double Border
        {
            get { return my_Border; }
            set
            {
                my_Border = value;
                MakeSegments();
            }
        }

        public double SegmentThickness
        {
            get { return my_SegmentThickness ; }
            set
            {
                my_SegmentThickness = value;
                MakeSegments();
            }
        }

        public double Campher
        {
            get { return my_Campher; }
            set
            {
                my_Campher = value;
                MakeSegments();
            }
        }

        public bool HasCampher
        {
            get { return my_HasCampher; }
            set
            {
                my_HasCampher = value;
                MakeSegments();
            }
        }

        public double SegmentSpace
        {
            get { return my_SegmentSpace ; }
            set
            {
                my_SegmentSpace = value;
                MakeSegments();
            }
        }

        public bool IsTilted
        {
            get { return my_IsTilted ; }
            set
            {
                my_IsTilted = value;
                MakeSegments();
            }
        }

        public bool ShowDot
        {
            get { return my_ShowDot; }
            set
            {
                my_ShowDot = value;
                MakeSegments();
            }
        }

        public Brush BackColor
        {
            get { return my_Backcolor; }
            set
            {
                my_Backcolor = value;
                MakeSegments();
            }
        }

        public int Value
        {
            get { return my_Value; }
            set
            {
                my_Value = value;
                if (my_Value > 9) { my_Value = 9; }
                if (my_Value < 0) { my_Value = 0; }
                //Set segment visibility to display the value
                for (int I = 0; I < 7; I++)
                {
                    if (HexCodes[my_Value][I] == '1')
                    {
                        Segments[I].Visibility = Visibility.Visible;
                    }
                    else
                    {
                        Segments[I].Visibility = Visibility.Hidden;
                    }
                }
                if (my_ShowDot)
                {
                    Dot.Visibility = Visibility.Visible;
                }
                else
                {
                    Dot.Visibility = Visibility.Hidden;
                }
            }
        }

        #endregion 

        private void MakeSegments()
        {
            double X;
            double Y;
            double DX = 0.0;
            double DX1;
            double DY1;
            PointCollection points;
            my_VertSegmentLength = (my_Height - 2 * my_Border - 3 * my_SegmentThickness - 4 * my_SegmentSpace) / 2;
            my_HorSegmentLength = my_Width - 2 * my_Border - 3 * my_SegmentThickness - 3 * my_SegmentSpace;
            DY1 = my_Height - my_Border - my_SegmentThickness - my_SegmentSpace; //Y of furthest tilted point
            DX1 = my_Width - 2 * my_Border - 2 * my_SegmentThickness - 2 * my_SegmentSpace - my_HorSegmentLength; //X shift of furthest tilted point
            //Display Background
            Back.Width = my_Width;
            Back.Height = my_Height;
            //segment A
            points = new PointCollection();
            X = my_Border + my_SegmentThickness + my_SegmentSpace;
            Y = my_Border;
            if (my_IsTilted) { DX = DX1 * (my_Height - Y) / DY1; }
            points.Add(new Point(X + DX, Y));
            X = X + my_HorSegmentLength;
            if (my_IsTilted) { DX = DX1 * (my_Height - Y) / DY1; }
            points.Add(new Point(X + DX, Y));
            X = X + my_Campher;
            Y = Y + my_SegmentThickness / 2;
            if (my_IsTilted) { DX = DX1 * (my_Height - Y) / DY1; }
            points.Add(new Point(X + DX, Y));
            X = X - my_Campher;
            Y = Y + my_SegmentThickness / 2;
            if (my_IsTilted ) { DX = DX1 * (my_Height - Y) / DY1; }
            points.Add(new Point(X + DX, Y));
            X = X - my_HorSegmentLength;
            if (my_IsTilted ) { DX = DX1 * (my_Height - Y) / DY1; }
            points.Add(new Point(X + DX, Y));
            X = X - my_Campher;
            Y = Y - my_SegmentThickness / 2;
            if (my_IsTilted ) { DX = DX1 * (my_Height - Y) / DY1; }
            points.Add(new Point(X + DX, Y));
            X = X + my_Campher;
            Y = Y - my_SegmentThickness / 2;
            if (my_IsTilted ) { DX = DX1 * (my_Height - Y) / DY1; }
            points.Add(new Point(X + DX, Y));
            Segments[0].Points = points;
            //segment B
            points = new PointCollection();
            X = my_Border + my_SegmentThickness + my_HorSegmentLength + 2 * my_SegmentSpace;
            Y = my_Border + my_SegmentThickness + my_SegmentSpace;
            if (my_IsTilted ) { DX = DX1 * (my_Height - Y) / DY1; }
            points.Add(new Point(X + DX, Y));
            X = X + my_SegmentThickness / 2;
            Y = Y - my_Campher;
            if (my_IsTilted ) { DX = DX1 * (my_Height - Y) / DY1; }
            points.Add(new Point(X + DX, Y));
            X = X + my_SegmentThickness / 2;
            Y = Y + my_Campher;
            if (my_IsTilted ) { DX = DX1 * (my_Height - Y) / DY1; }
            points.Add(new Point(X + DX, Y));
            Y = Y + my_VertSegmentLength;
            if (my_IsTilted ) { DX = DX1 * (my_Height - Y) / DY1; }
            points.Add(new Point(X + DX, Y));
            X = X - my_SegmentThickness / 2;
            Y = Y + my_Campher;
            if (my_IsTilted ) { DX = DX1 * (my_Height - Y) / DY1; }
            points.Add(new Point(X + DX, Y));
            X = X - my_SegmentThickness / 2;
            Y = Y - my_Campher;
            if (my_IsTilted ) { DX = DX1 * (my_Height - Y) / DY1; }
            points.Add(new Point(X + DX, Y));
            Y = Y - my_VertSegmentLength;
            if ( my_IsTilted ) { DX = DX1 * (my_Height - Y) / DY1; }
            points.Add(new Point(X + DX, Y));
            Segments[1].Points = points;
            //segment C
            points = new PointCollection();
            X = my_Border + my_SegmentThickness + my_HorSegmentLength + 2 * my_SegmentSpace;
            Y = my_Border + 2 * my_SegmentThickness + 3 * my_SegmentSpace + my_VertSegmentLength;
            if ( my_IsTilted ) { DX = DX1 * (my_Height - Y) / DY1; }
            points.Add(new Point(X + DX, Y));
            X = X + my_SegmentThickness / 2;
            Y = Y - my_Campher;
            if ( my_IsTilted ) { DX = DX1 * (my_Height - Y) / DY1; }
            points.Add(new Point(X + DX, Y));
            X = X + my_SegmentThickness / 2;
            Y = Y + my_Campher;
            if ( my_IsTilted ) { DX = DX1 * (my_Height - Y) / DY1; }
            points.Add(new Point(X + DX, Y));
            Y = Y + my_VertSegmentLength;
            if ( my_IsTilted ) { DX = DX1 * (my_Height - Y) / DY1; }
            points.Add(new Point(X + DX, Y));
            X = X - my_SegmentThickness / 2;
            Y = Y + my_Campher;
            if ( my_IsTilted ) { DX = DX1 * (my_Height - Y) / DY1; }
            points.Add(new Point(X + DX, Y));
            X = X - my_SegmentThickness / 2;
            Y = Y - my_Campher;
            if ( my_IsTilted ) { DX = DX1 * (my_Height - Y) / DY1; }
            points.Add(new Point(X + DX, Y));
            Y = Y - my_VertSegmentLength;
            if ( my_IsTilted ) { DX = DX1 * (my_Height - Y) / DY1; }
            points.Add(new Point(X + DX, Y));
            Segments[2].Points = points;
            //segment D
            points = new PointCollection();
            X = my_Border + my_SegmentThickness + my_SegmentSpace;
            Y = my_Border + 2 * my_SegmentThickness + 2 * my_VertSegmentLength + 4 * my_SegmentSpace;
            if ( my_IsTilted ) { DX = DX1 * (my_Height - Y) / DY1; }
            points.Add(new Point(X + DX, Y));
            X = X + my_HorSegmentLength;
            if ( my_IsTilted ) { DX = DX1 * (my_Height - Y) / DY1; }
            points.Add(new Point(X + DX, Y));
            X = X + my_Campher;
            Y = Y + my_SegmentThickness / 2;
            if ( my_IsTilted ) { DX = DX1 * (my_Height - Y) / DY1; }
            points.Add(new Point(X + DX, Y));
            X = X - my_Campher;
            Y = Y + my_SegmentThickness / 2;
            if ( my_IsTilted ) { DX = DX1 * (my_Height - Y) / DY1; }
            points.Add(new Point(X + DX, Y));
            X = X - my_HorSegmentLength;
            if ( my_IsTilted ) { DX = DX1 * (my_Height - Y) / DY1; }
            points.Add(new Point(X + DX, Y));
            X = X - my_Campher;
            Y = Y - my_SegmentThickness / 2;
            if ( my_IsTilted ) { DX = DX1 * (my_Height - Y) / DY1; }
            points.Add(new Point(X + DX, Y));
            X = X + my_Campher;
            Y = Y - my_SegmentThickness / 2;
            if ( my_IsTilted ) { DX = DX1 * (my_Height - Y) / DY1; }
            points.Add(new Point(X + DX, Y));
            Segments[3].Points = points;
            //segment E
            points = new PointCollection();
            X = my_Border;
            Y = my_Border + 2 * my_SegmentThickness + 3 * my_SegmentSpace + my_VertSegmentLength;
            if ( my_IsTilted ) { DX = DX1 * (my_Height - Y) / DY1; }
            points.Add(new Point(X + DX, Y));
            X = X + my_SegmentThickness / 2;
            Y = Y - my_Campher;
            if ( my_IsTilted ) { DX = DX1 * (my_Height - Y) / DY1; }
            points.Add(new Point(X + DX, Y));
            X = X + my_SegmentThickness / 2;
            Y = Y + my_Campher;
            if ( my_IsTilted ) { DX = DX1 * (my_Height - Y) / DY1; }
            points.Add(new Point(X + DX, Y));
            Y = Y + my_VertSegmentLength;
            if ( my_IsTilted ) { DX = DX1 * (my_Height - Y) / DY1; }
            points.Add(new Point(X + DX, Y));
            X = X - my_SegmentThickness / 2;
            Y = Y + my_Campher;
            if ( my_IsTilted ) { DX = DX1 * (my_Height - Y) / DY1; }
            points.Add(new Point(X + DX, Y));
            X = X - my_SegmentThickness / 2;
            Y = Y - my_Campher;
            if ( my_IsTilted ) { DX = DX1 * (my_Height - Y) / DY1; }
            points.Add(new Point(X + DX, Y));
            Y = Y - my_VertSegmentLength;
            if ( my_IsTilted ) { DX = DX1 * (my_Height - Y) / DY1; }
            points.Add(new Point(X + DX, Y));
            Segments[4].Points = points;
            //segment F
            points = new PointCollection();
            X = my_Border;
            Y = my_Border + my_SegmentThickness + my_SegmentSpace;
            if ( my_IsTilted ) { DX = DX1 * (my_Height - Y) / DY1; }
            points.Add(new Point(X + DX, Y));
            X = X + my_SegmentThickness / 2;
            Y = Y - my_Campher;
            if ( my_IsTilted ) { DX = DX1 * (my_Height - Y) / DY1; }
            points.Add(new Point(X + DX, Y));
            X = X + my_SegmentThickness / 2;
            Y = Y + my_Campher;
            if ( my_IsTilted ) { DX = DX1 * (my_Height - Y) / DY1; }
            points.Add(new Point(X + DX, Y));
            Y = Y + my_VertSegmentLength;
            if ( my_IsTilted ) { DX = DX1 * (my_Height - Y) / DY1; }
            points.Add(new Point(X + DX, Y));
            X = X - my_SegmentThickness / 2;
            Y = Y + my_Campher;
            if ( my_IsTilted ) { DX = DX1 * (my_Height - Y) / DY1; }
            points.Add(new Point(X + DX, Y));
            X = X - my_SegmentThickness / 2;
            Y = Y - my_Campher;
            if ( my_IsTilted ) { DX = DX1 * (my_Height - Y) / DY1; }
            points.Add(new Point(X + DX, Y));
            Y = Y - my_VertSegmentLength;
            if ( my_IsTilted ) { DX = DX1 * (my_Height - Y) / DY1; }
            points.Add(new Point(X + DX, Y));
            Segments[5].Points = points;
            //segment G
            points = new PointCollection();
            X = my_Border + my_SegmentThickness + my_SegmentSpace;
            Y = my_Border + my_SegmentThickness + my_VertSegmentLength + 2 * my_SegmentSpace;
            if ( my_IsTilted ) { DX = DX1 * (my_Height - Y) / DY1; }
            points.Add(new Point(X + DX, Y));
            X = X + my_HorSegmentLength;
            if ( my_IsTilted ) { DX = DX1 * (my_Height - Y) / DY1; }
            points.Add(new Point(X + DX, Y));
            X = X + my_Campher;
            Y = Y + my_SegmentThickness / 2;
            if ( my_IsTilted ) { DX = DX1 * (my_Height - Y) / DY1; }
            points.Add(new Point(X + DX, Y));
            X = X - my_Campher;
            Y = Y + my_SegmentThickness / 2;
            if ( my_IsTilted ) { DX = DX1 * (my_Height - Y) / DY1; }
            points.Add(new Point(X + DX, Y));
            X = X - my_HorSegmentLength;
            if ( my_IsTilted ) { DX = DX1 * (my_Height - Y) / DY1; }
            points.Add(new Point(X + DX, Y));
            X = X - my_Campher;
            Y = Y - my_SegmentThickness / 2;
            if ( my_IsTilted ) { DX = DX1 * (my_Height - Y) / DY1; }
            points.Add(new Point(X + DX, Y));
            X = X + my_Campher;
            Y = Y - my_SegmentThickness / 2;
            if ( my_IsTilted ) { DX = DX1 * (my_Height - Y) / DY1; }
            points.Add(new Point(X + DX, Y));
            Segments[6].Points = points;
            //Decimal Point
            X = my_Width - my_Border - my_SegmentThickness;
            Y = my_Height - my_Border - my_SegmentThickness;
            Dot.Width = my_SegmentThickness;
            Dot.Height = my_SegmentThickness;
            Dot.Fill = my_SegmentColor;
            Dot.SetValue(Canvas.LeftProperty, X + my_Left);
            Dot.SetValue(Canvas.TopProperty, Y + my_Top);
            //Set the colors and the position of the display in the canvas.
            Back.Fill = my_Backcolor;
            Back.SetValue(Canvas.LeftProperty, my_Left);
            Back.SetValue(Canvas.TopProperty, my_Top);
            for (int I = 0; I < 7; I++)
            {
                Segments[I].SetValue(Canvas.LeftProperty, my_Left);
                Segments[I].SetValue(Canvas.TopProperty, my_Top);
                Segments[I].Fill = my_SegmentColor;
            }
        }

        public void Draw(Canvas c)
        {
            Back.SetValue(Canvas.LeftProperty, my_Left);
            Back.SetValue(Canvas.TopProperty, my_Top);
            c.Children.Add(Back);
            for (int I = 0; I < 7; I++)
            {
                Segments[I].SetValue(Canvas.LeftProperty, my_Left);
                Segments[I].SetValue(Canvas.TopProperty, my_Top);
                Segments[I].Fill = my_SegmentColor;
                c.Children.Add(Segments[I]);
            }
            Dot.Fill = my_SegmentColor;
            c.Children.Add(Dot);
        }

        public void SetDefault()
        {
            my_SegmentColor = Brushes.Red;
            my_Backcolor = Brushes.Black;
            my_Border = (int)(3 * my_Height / 100);
            my_SegmentThickness = (int)(10 * my_Height / 100);
            my_HasCampher = true;
            my_Campher = (int)(my_SegmentThickness / 2);
            my_SegmentSpace = (int)(my_Height / 100);
            my_IsTilted = true;
            MakeSegments();
        }
    }
}
