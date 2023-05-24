using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Flappy_Bird_with_NeuralNet
{
    internal class Bird
    {
        private double my_X;
        private double my_Y;
        private double my_Speed;
        private double my_Upspeed;
        private double my_Size;
        private bool my_Alive;
        private int my_Score;
        private double my_Fitness;
        private Ellipse my_ellipse;
        private Canvas my_Canvas;
        private NeuralNet my_Brain;

        public Bird(double X, double Y, double Size, Canvas Can)
        {
            my_X = X;
            my_Y = Y;
            my_Size = Size;
            my_Canvas = Can;
            my_Speed = 0.0;
            my_Score = 0;
            my_Fitness = 0.0;
            my_Alive = true;
            my_ellipse = new Ellipse()
            {
                Width = my_Size,
                Height = my_Size,
                Fill = Brushes.Red
            };
            my_ellipse.SetValue(Canvas.LeftProperty, my_X - my_Size / 2);
            my_ellipse.SetValue(Canvas.TopProperty, my_Y - my_Size / 2);
        }

        public void SetBrain(NeuralNet NN)
        {
            my_Brain = NN.Copy();
        }

        public double Y
        {
            get { return my_Y; }
            set { my_Y = value; }
        }

        public bool Alive
        {
            get { return my_Alive; }
            set { my_Alive = value; }
        }

        public double UpSpeed
        {
            get { return my_Upspeed; }
            set { my_Upspeed = value; }
        }

        public double Speed
        {
            get { return my_Speed; }
            set { my_Speed = value; }
        }

        public NeuralNet Brain
        {
            get { return my_Brain; }
        }

        public int Score
        {
            get { return my_Score; }
            set { my_Score = value; }
        }

        public double Fitness
        {
            get { return my_Fitness; }
            set { my_Fitness = value; }
        }

        public void Draw()
        {
            my_Canvas.Children.Add(my_ellipse);
        }

        public void Remove()
        {
            my_Canvas.Children.Remove(my_ellipse);
        }

        public void Think(Gate g)
        {
            double[] inputs = new double[5];
            double[] output;
            inputs[0] = my_Y / my_Canvas.ActualHeight;
            inputs[1] = g.GateBottom / my_Canvas.ActualHeight;
            inputs[2] = g.GateTop / my_Canvas.ActualHeight;
            inputs[3] = 0.5 * Math.Abs(g.X - my_X) / my_Canvas.ActualWidth;
            inputs[4] = my_Speed / 10;
            output = my_Brain.Query(inputs);
            if (output[0] > output[1])
            {
                my_Speed -= my_Upspeed;  //Flap;
            }
        }

        public void Update(double downspeed)
        {
            my_Speed += downspeed;
            my_Y += my_Speed;
            if (my_Y > my_Canvas.ActualHeight - my_Size) //Die when hit the bottom
            {
                my_Y = my_Canvas.ActualHeight - my_Size;
                my_Alive = false;     
            }
            if (my_Y < my_Size) //Die when hit the top
            {
                my_Y = my_Size;
                my_Alive = false;    
            }
            my_ellipse.SetValue(Canvas.TopProperty, my_Y - my_Size / 2);
        }

        public void CheckCollision(Gate g)
        {
            if (my_Y < g.GateTop + my_Size / 2 || my_Y > g.GateBottom - my_Size / 2) //Die when hit a gate
            {
                my_Alive = false;     
                //Improve the score for dying close to the gate
                my_Score += (int)(my_Canvas.ActualHeight - Math.Abs((g.GateTop + g.GateBottom) / 2 - my_Y));
            }
        }

        public Bird copy()
        {
            Bird result = new Bird(my_X, my_Canvas.ActualHeight / 2, my_Size, my_Canvas);
            result.SetBrain(my_Brain.Copy());
            result.UpSpeed = my_Upspeed;
            return result;
        }

        public void SaveNN(string file)
        {
            my_Brain.SaveToFile(file);
        }

        public void LoadNN(string file)
        {
            my_Brain = NeuralNet.LoadFromFile(file);
        }
    }
}
