using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Plinko
{
    public partial class MainWindow : Window
    {
        public delegate void WaitDelegate(int t);
        private bool AppStarted = false;
        private int WaitTime = 10;
        private Random rnd = new Random();
        private int PinsPerRow = 10;
        private List<PlinkoBall> balls;
        private List<PlinkoPin> pins;
        private List<PlinkoSlot> slots;
        private double pinDiameter = 13;
        private double ballDiameter = 25.0;
        private Vector gravity = new Vector(0.0, 0.1);
        private double elasticity = 50.0;  //% of kinetic energy remaining after collision
        private int frameCounter = 0;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            PlinkoPin pin;
            Line slotdivider;
            PlinkoSlot slot;
            Vector pos;
            double Xdist = canvas1.ActualWidth / (PinsPerRow + 1);
            pins = new List<PlinkoPin>();
            balls = new List<PlinkoBall>();
            slots = new List<PlinkoSlot>();
            //Make the pins
            for (int row = 1; row <= 10; row++)
            {
                pos = new Vector();
                pos.Y = row * (canvas1.ActualHeight - 180) / 10 + 50;
                if (row % 2 == 0)
                {
                    pos.X = 0;
                    for (int col = 1; col <= PinsPerRow; col++)
                    {
                        pos.X += Xdist;
                        pin = new PlinkoPin(pinDiameter, pos);
                        pins.Add(pin);
                        pin.Draw(canvas1);
                    }
                }
                else
                {
                    pos.X = Xdist / 2;
                    for (int col = 1; col < PinsPerRow; col++)
                    {
                        pos.X += Xdist;
                        pin = new PlinkoPin(pinDiameter, pos);
                        pins.Add(pin);
                        pin.Draw(canvas1);
                    }
                }
            }
            //Make the slots at the bottom
            for (int I = 0; I <= PinsPerRow; I++)
            {
                slot = new PlinkoSlot(I * Xdist, (I + 1) * Xdist);
                slots.Add(slot);
                slot.Draw(canvas1);
                if (I < PinsPerRow)
                {
                    slotdivider = new Line()
                    {
                        Stroke = Brushes.Black,
                        StrokeThickness = 8.0,
                        X1 = slot.Right,
                        Y1 = canvas1.ActualHeight,
                        X2 = slot.Right,
                        Y2 = pos.Y + 20
                    };
                    canvas1.Children.Add(slotdivider);
                }
            }
        }

        private void NewBall()
        {
            PlinkoBall ball = new PlinkoBall(ballDiameter, new Vector(canvas1.ActualWidth / 2 + (pinDiameter + ballDiameter - 1) * (rnd.NextDouble() - 0.5), 20.0));
            balls.Add(ball);
            ball.Draw(canvas1);
        }

        private void Render()
        {
            do
            {
                for (int I = balls.Count - 1; I >= 0; I--)
                {
                    balls[I].Update(gravity, elasticity, pins);
                    if (balls[I].Position.Y >= canvas1.ActualHeight - 100)
                    {
                        balls[I].Lock();
                    }
                    if (balls[I].Position.Y >= canvas1.ActualHeight - ballDiameter / 2)
                    {
                        for (int J = 0; J < slots.Count; J++)
                        {
                            if (balls[I].Position.X > slots[J].Left & balls[I].Position.X < slots[J].Right)
                            {
                                slots[J].Count += 1;
                            }
                        }
                        balls[I].Remove(canvas1);
                        balls.RemoveAt(I);
                    }
                }
                int maxCount = 0;
                for (int I = 0; I < slots.Count; I++)
                {
                    if (slots[I].Count > maxCount) maxCount = slots[I].Count;
                }
                for (int I = 0; I < slots.Count; I++)
                {
                    if (slots[I].Count > 0) slots[I].SetHeight(100, maxCount);
                }
                Dispatcher.Invoke(DispatcherPriority.ApplicationIdle, new WaitDelegate(Wait), WaitTime);
                frameCounter += 1;
                if (frameCounter == 100)
                {
                    frameCounter = 0;
                    NewBall();
                }
            } while (AppStarted);
        }

        private void Wait(int t)
        {
            Thread.Sleep(t);
        }

        private void Window_Closing(Object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            if (!AppStarted) 
            {
                BtnStart.Content = "STOP";
                AppStarted = true;
                NewBall();
                Render();
            }
            else
            {
                BtnStart.Content = "START";
                AppStarted = false;
            }
        }
    }
}