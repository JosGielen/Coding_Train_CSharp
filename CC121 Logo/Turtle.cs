using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Logo
{
    class Turtle
    {
        private readonly double my_StartX;
        private readonly double my_StartY;
        private double my_X;
        private double my_Y;
        private double my_Angle;
        private Brush my_Color;
        private double my_Size;
        private bool my_Drawing;
        private readonly Canvas my_Canvas;

        public Turtle(Point Startlocation, Canvas canvas)
        {
            my_StartX = Startlocation.X;
            my_StartY = Startlocation.Y;
            my_X = Startlocation.X;
            my_Y = Startlocation.Y;
            my_Drawing = true;
            my_Canvas = canvas;
            my_Color = Brushes.Black;
            my_Size = 1.0;
        }

        public double X
        {
            get { return my_X; }
            set { my_X = value; }
        }

        public double Y
        {
            get { return my_Y; }
            set { my_Y = value; }
        }

        public Brush LineColor
        {
            get { return my_Color; }
            set { my_Color = value; }
        }

        public double Size
        {
            get { return my_Size; }
            set { my_Size = value; }
        }

        public double Angle
        {
            get { return my_Angle; }
            set { my_Angle = value; }
        }

        public void Reset()
        {
            my_X = my_StartX;
            my_Y = my_StartY;
            my_Drawing = true;
            my_Color = Brushes.Black;
            my_Size = 1.0;
            my_Angle = 0.0;
        }

        public void ExecuteCmd(LogoCommand cmd)
        {
            double endX;
            double endY;
            Line l;
            switch (cmd.Command)
            {
                case "fd":
                    endX = my_X + cmd.Value * Math.Cos(my_Angle);
                    endY = my_Y + cmd.Value * Math.Sin(my_Angle);
                    if (my_Drawing)
                    {
                        l = new Line
                        {
                            Stroke = my_Color,
                            StrokeThickness = my_Size,
                            X1 = my_X,
                            Y1 = my_Y,
                            X2 = endX,
                            Y2 = endY
                        };
                        my_Canvas.Children.Add(l);
                    }
                    my_X = endX;
                    my_Y = endY;
                    break;
                case "bd":
                    endX = my_X - cmd.Value * Math.Cos(my_Angle);
                    endY = my_Y - cmd.Value * Math.Sin(my_Angle);
                    if (my_Drawing)
                    {
                        l = new Line
                        {
                            Stroke = my_Color,
                            StrokeThickness = my_Size,
                            X1 = my_X,
                            Y1 = my_Y,
                            X2 = endX,
                            Y2 = endY
                        };
                        my_Canvas.Children.Add(l);
                    }
                    my_X = endX;
                    my_Y = endY;
                    break;
                case "rt":
                    my_Angle += cmd.Value * Math.PI / 180;
                    break;
                case "lt":
                    my_Angle -= cmd.Value * Math.PI / 180;
                    break;
                case "pu":
                    my_Drawing = false;
                    break;
                case "pd":
                    my_Drawing = true;
                    break;
                case "col":
                    string[] parts = cmd.Parameter.Split(',');
                    try
                    {
                        byte r = byte.Parse(parts[0]);
                        byte g = byte.Parse(parts[1]);
                        byte b = byte.Parse(parts[2]);
                        my_Color = new SolidColorBrush(Color.FromRgb(r, g, b));
                    }
                    catch
                    {
                        //Do nothing
                    }
                    break;
                case "size":
                    my_Size = cmd.Value;
                    break;
                case "setx":
                    double newX;
                    try
                    {
                        newX = cmd.Value;
                    }
                    catch
                    {
                        return;
                    }
                    my_X = newX;
                    break;
                case "sety":
                    double newY;
                    try
                    {
                        newY = cmd.Value;
                    }
                    catch
                    {
                        return;
                    }
                    my_Y = newY;
                    break;
                case "home":
                    my_X = my_StartX;
                    my_Y = my_StartY;
                    break;
            }
        }
    }
}
