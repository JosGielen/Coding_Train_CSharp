using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Ulam_Spiral
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private double StepSize = 7;
        private int number;
        private int maxNumber;
        private int StepCounter; //Number of steps in the current direction
        private int DirSize;     //Max number of steps in the current direction
        private int TurnCounter; //Increase DirSize every 2 turns
        private int StepDir;     //Current direction: //1 = Right; 2 = Up; 3 = Left; 4 = Down 
        private Point PreviousPt;
        private Point CurrentPt;
        private bool ShowLine = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            number = 1;
            maxNumber = (int)(canvas1.ActualWidth / StepSize * (canvas1.ActualHeight / StepSize + 1)) + 1;
            StepCounter = 0;
            DirSize = 1;
            TurnCounter = 0;
            StepDir = 1;
            PreviousPt = new Point(canvas1.ActualWidth / 2, canvas1.ActualHeight / 2);
            CurrentPt = new Point(canvas1.ActualWidth / 2, canvas1.ActualHeight / 2);
            Render();
        }

        private void Render()
        {
            Ellipse El;
            Line L;
            while (true)
            {
                El = new Ellipse()
                {
                    Width = 6,
                    Height = 6,
                    Stroke = Brushes.Black,
                    StrokeThickness = 1
                };
                El.SetValue(Canvas.TopProperty, CurrentPt.Y - El.Height / 2);
                El.SetValue(Canvas.LeftProperty, CurrentPt.X - El.Width / 2);
                if (IsPrime(number))
                {
                    El.Fill = Brushes.Black;
                }
                canvas1.Children.Add(El);
                if (ShowLine)
                {
                    L = new Line()
                    {
                        X1 = PreviousPt.X,
                        Y1 = PreviousPt.Y,
                        X2 = CurrentPt.X,
                        Y2 = CurrentPt.Y,
                        Stroke = Brushes.Black,
                        StrokeThickness = 1.0
                    };
                    canvas1.Children.Add(L);
                }
                number++;
                if (StepCounter == DirSize) //Need to turn
                {
                    StepDir++;
                    if (StepDir > 4) StepDir = 1;
                    StepCounter = 0;
                    TurnCounter++;
                    if (TurnCounter % 2 == 0) DirSize++;
                }
                PreviousPt = CurrentPt;
                switch (StepDir)
                {
                    case 1:
                    {
                        CurrentPt = new Point(CurrentPt.X + StepSize, CurrentPt.Y);
                        break;
                    }
                    case 2:
                    {
                        CurrentPt = new Point(CurrentPt.X, CurrentPt.Y - StepSize);
                        break;
                    }
                    case 3:
                    {
                        CurrentPt = new Point(CurrentPt.X - StepSize, CurrentPt.Y);
                        break;
                    }
                    case 4:
                    {
                        CurrentPt = new Point(CurrentPt.X, CurrentPt.Y + StepSize);
                        break;
                    }
                }
                StepCounter++;
                Dispatcher.Invoke(Wait, System.Windows.Threading.DispatcherPriority.ApplicationIdle);
                if (number > maxNumber) return;
            }
        }

        private bool IsPrime(int n)
        {
            for (int I = 2; I <= Math.Sqrt(n); I++)
            {
                if (n % I == 0) return false;
            }
            return true;
        }

        private void Wait()
        {
            Thread.Sleep(1);
        }
    }
}
