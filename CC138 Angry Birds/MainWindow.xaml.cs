using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Box2D.NET;

namespace Angry_Birds
{
    public partial class MainWindow : Window
    {
        private bool mouseDown = false;
        private Vector mousePos;
        private Vector Force;
        private double floorHeight = 40;
        private double poleHeight = 200;
        private Ellipse dummyBall;
        private Vector dummyBallLoc;
        private double ballRadius = 20.0;
        private Ball ball;
        private Rectangle pole;
        private Vector poleLocation;
        private Line rubberBand;
        private List<Ball> myBalls;
        private List<Box> myBoxes;
        //Box2D settings
        private B2WorldId worldId;
        private float timeStep = 1f / 60.0f;
        private int subStepCount = 4;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            myBoxes = new List<Box>();
            myBalls = new List<Ball>();
            //Create a Box2D World
            B2WorldDef worldDef = B2Types.b2DefaultWorldDef();
            worldDef.gravity = new B2Vec2(0.0f, 9.81f);
            worldId = B2Worlds.b2CreateWorld(worldDef);
            //Create the floor
            Vector floorLoc = new Vector(canvas1.ActualWidth / 2, canvas1.ActualHeight - floorHeight / 2);
            Size floorSize = new Size(canvas1.ActualWidth, floorHeight);
            Box floor = new Box(floorLoc, floorSize, true, worldId);
            floor.Rect.Fill = Brushes.Green;
            floor.Draw(canvas1);
            //Create the slingshot pole
            poleLocation = new Vector(400.0, canvas1.ActualHeight - poleHeight - floorHeight);
            pole = new Rectangle()
            {
                Width = 15.0,
                Height = poleHeight,
                Stroke = Brushes.Black,
                StrokeThickness = 1.0,
                Fill = Brushes.Brown,
            };
            pole.SetValue(Canvas.LeftProperty, poleLocation.X - pole.Width);
            pole.SetValue(Canvas.TopProperty, poleLocation.Y);
            canvas1.Children.Add(pole);
            LoadSlingShot();
            CreateTargetBoxes();
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        private void CreateTargetBoxes()
        {
            Size boxSize = new Size(30, 40);   
            for (int i = 0; i < 10; i++)
            {
                Vector loc = new Vector(canvas1.ActualWidth - 200, canvas1.ActualHeight - floorHeight - (i + 0.5) * boxSize.Height);
                Box newBox = new Box(loc, boxSize, false, worldId);
                newBox.FillColor = Brushes.DarkGoldenrod;
                newBox.Draw(canvas1);
                myBoxes.Add(newBox);
            }
            for (int i = 0; i < 6; i++)
            {
                Vector loc = new Vector(canvas1.ActualWidth - 400, canvas1.ActualHeight - floorHeight - (i + 0.5) * boxSize.Height);
                Box newBox = new Box(loc, boxSize, false, worldId);
                newBox.FillColor = Brushes.DarkGoldenrod;
                newBox.Draw(canvas1);
                myBoxes.Add(newBox);
            }
            for (int i = 0; i < 4; i++)
            {
                Vector loc = new Vector(canvas1.ActualWidth - 600, canvas1.ActualHeight - floorHeight - (i + 0.5) * boxSize.Height);
                Box newBox = new Box(loc, boxSize, false, worldId);
                newBox.FillColor = Brushes.DarkGoldenrod;
                newBox.Draw(canvas1);
                myBoxes.Add(newBox);
            }
        }

        private void LoadSlingShot()
        {
            //Create the dummyBall
            dummyBallLoc = new Vector(300.0, canvas1.ActualHeight - 40 - ballRadius);
            dummyBall = new Ellipse()
            {
                Width = 2 * ballRadius,
                Height = 2 * ballRadius,
                Stroke = Brushes.Black,
                StrokeThickness = 1.0,
                Fill = Brushes.Red,
            };
            dummyBall.SetValue(Canvas.LeftProperty, dummyBallLoc.X - ballRadius);
            dummyBall.SetValue(Canvas.TopProperty, dummyBallLoc.Y - ballRadius);
            canvas1.Children.Add(dummyBall);
            //Create the slingshot rubberband
            rubberBand = new Line()
            {
                X1 = dummyBallLoc.X,
                Y1 = dummyBallLoc.Y,
                X2 = poleLocation.X,
                Y2 = poleLocation.Y,
                Stroke = Brushes.Beige,
                StrokeThickness = 5.0
            };
            canvas1.Children.Add(rubberBand);

        }

        private void CompositionTarget_Rendering(object? sender, EventArgs e)
        {
            //Update the Box2D world
            B2Worlds.b2World_Step(worldId, timeStep, subStepCount);
            //Update all the Balls
            for (int i = myBalls.Count - 1; i >= 0; i--)
            {
                myBalls[i].Update();
                //Remove Boxes that slide off the side of the canvas
                myBalls[i].Edges(canvas1);
                if (!myBalls[i].alive)
                {
                    myBalls[i].Remove(canvas1);
                    myBalls.RemoveAt(i);
                }
            }
            //Update all the Boxes
            for (int i = myBoxes.Count - 1; i >= 0; i--)
            {
                myBoxes[i].Update();
                //Remove Boxes that slide off the side of the canvas
                myBoxes[i].Edges(canvas1);
                if (!myBoxes[i].alive)
                {
                    myBoxes[i].Remove(canvas1);
                    myBoxes.RemoveAt(i);
                }
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //Check if the ball is selected
            Point pt = e.GetPosition(canvas1);
            mousePos = new Vector(pt.X, pt.Y);
            if ((dummyBallLoc - mousePos).Length < 10)
            {
                mouseDown = true;
            }
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                //Move the dummyball and the rubberband
                Point pt = e.GetPosition(canvas1);
                mousePos = new Vector(pt.X, pt.Y);
                if (mousePos.Y > canvas1.ActualHeight - 60) { mousePos.Y = canvas1.ActualHeight - 60; }
                dummyBall.SetValue(Canvas.LeftProperty, mousePos.X - ballRadius);
                dummyBall.SetValue(Canvas.TopProperty, mousePos.Y - ballRadius);
                rubberBand.X1 = mousePos.X;
                rubberBand.Y1 = mousePos.Y;
            }
        }

        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (mouseDown)
            {
                mouseDown = false;
                //Calculate the slingshot force
                Point pt = e.GetPosition(canvas1);
                mousePos = new Vector(pt.X, pt.Y);
                Force = new Vector(poleLocation.X, poleLocation.Y - floorHeight)  - mousePos;
                Force *= 400;
                //Create the real ball
                double ballRadius = 20.0;
                ball = new Ball(mousePos, ballRadius, false, worldId);
                ball.FillColor = Brushes.Red;
                ball.Draw(canvas1);
                B2Bodies.b2Body_ApplyForceToCenter(ball.my_ID, Utilities.Vector2Vec(Force), true);
                myBalls.Add(ball);
                //Remove the dummyBall and the rubberband
                if (canvas1.Children.Contains(dummyBall))
                {
                    canvas1.Children.Remove(dummyBall);
                }
                if (canvas1.Children.Contains(rubberBand))
                {
                    canvas1.Children.Remove(rubberBand);
                }
            }
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                LoadSlingShot();
            }
            if (e.Key == Key.Escape)
            {
                //Remove all the Balls
                for (int i = myBalls.Count - 1; i >= 0; i--)
                {
                    myBalls[i].Remove(canvas1);
                    myBalls.RemoveAt(i);
                }
                //Remove all the Boxes
                for (int i = myBoxes.Count - 1; i >= 0; i--)
                {
                    myBoxes[i].Remove(canvas1);
                    myBoxes.RemoveAt(i);
                }
                //Reset the game
                CreateTargetBoxes();
                LoadSlingShot();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}