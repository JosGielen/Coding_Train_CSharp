using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Flappy_Bird_with_NeuralNet
{
    internal class Gate
    {
        private double my_X;
        private double my_W;
        private double my_Space;
        private double my_UpperY;
        private double my_LowerY;
        private Rectangle my_Rect1;
        private Rectangle my_Rect2;
        private Canvas my_Canvas;
        static Random Rnd = new Random();

        public Gate(double X, double W, double space, Canvas can)
        {
            my_X = X;
            my_W = W;
            my_Space = space;
            my_Canvas = can;
            my_UpperY = (0.8 * can.ActualHeight - space) * Rnd.NextDouble() + 0.1 * can.ActualHeight; //between 0.1*Height and (0.9*Height - space);
            my_LowerY = my_UpperY + space;
            my_Rect1 = new Rectangle()
            {
                Width = my_W,
                Height = my_UpperY,
                Fill = Brushes.Yellow
            };
            my_Rect2 = new Rectangle()
            {
                Width = my_W,
                Height = my_Canvas.ActualHeight - my_LowerY,
                Fill = Brushes.Yellow
            };
            my_Rect1.SetValue(Canvas.LeftProperty, my_X);
            my_Rect1.SetValue(Canvas.TopProperty, 0.0);
            my_Rect2.SetValue(Canvas.LeftProperty, my_X);
            my_Rect2.SetValue(Canvas.TopProperty, my_LowerY);
        }

        public double X
        {
            get { return my_X; }
        }

        public double GateTop
        {
            get { return my_UpperY; }
            set 
            {
                my_UpperY = value;
                my_LowerY = my_UpperY + my_Space;
                my_Rect1 = new Rectangle()
                {
                    Width = my_W,
                    Height = my_UpperY,
                    Fill = Brushes.Yellow
                };
                my_Rect2 = new Rectangle()
                {
                    Width = my_W,
                    Height = my_Canvas.ActualHeight - my_LowerY,
                    Fill = Brushes.Yellow
                };
                my_Rect1.SetValue(Canvas.LeftProperty, my_X);
                my_Rect1.SetValue(Canvas.TopProperty, 0.0);
                my_Rect2.SetValue(Canvas.LeftProperty, my_X);
                my_Rect2.SetValue(Canvas.TopProperty, my_LowerY);
            }
        }

        public double GateBottom
        {
            get { return my_LowerY; }
        }

        public void Draw()
        {
            my_Canvas.Children.Add(my_Rect1);
            my_Canvas.Children.Add(my_Rect2);
        }

        public void Update(double speed)
        {
            my_X -= speed;
            my_Rect1.SetValue(Canvas.LeftProperty, my_X);
            my_Rect2.SetValue(Canvas.LeftProperty, my_X);
        }

        public void Reset()
        {
            my_X = my_Canvas.ActualWidth;
            my_UpperY = (0.8 * my_Canvas.ActualHeight - my_Space) * Rnd.NextDouble() + 0.1 * my_Canvas.ActualHeight; //between 0.1*Height and (0.9*Height - space);
            my_LowerY = my_UpperY + my_Space;
            my_Rect1.Height = my_UpperY;
            my_Rect2.Height = my_Canvas.ActualHeight - my_LowerY;
            my_Rect1.SetValue(Canvas.LeftProperty, my_X);
            my_Rect2.SetValue(Canvas.LeftProperty, my_X);
            my_Rect2.SetValue(Canvas.TopProperty, my_LowerY);
        }
    }
}
