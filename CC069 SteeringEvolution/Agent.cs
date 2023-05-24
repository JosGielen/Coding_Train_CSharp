using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SteeringEvolution
{
    class Agent
    {
        private static Random Rnd = new Random();
        private Canvas my_Canvas;
        private Polygon my_Poly = new Polygon();
        private Ellipse my_FoodVisual;
        private Ellipse my_PoisonVisual;
        private RotateTransform my_RT = new RotateTransform();
        private ColorPalette my_palette;
        private List<Brush> my_colors;
        private double EdgeDistance = 30;
        public Vector Location;
        public Vector Velocity;
        public Vector Acceleration;
        public Vector Force;
        public double my_Mass;
        public double my_MaxSpeed;
        public double my_MaxForce;
        public double[] DNA = new double[4];
        public double Health;
        public int Age;
        public double EatDistance = 4.0;

        public Agent(double locX, double locY, double mass, double maxSpeed, double maxForce)
        {
            Location = new Vector(locX, locY);
            Velocity = new Vector(Rnd.NextDouble(), Rnd.NextDouble());
            Acceleration = new Vector();
            my_Mass = mass;
            my_MaxSpeed = maxSpeed;
            my_MaxForce = maxForce;
            Health = 100;
            Age = 0;
            my_RT.Angle = 0;
            DNA[0] = Rnd.NextDouble() - 0.5; //Food attraction;
            DNA[1] = Rnd.NextDouble() - 0.5; //Poison atrtaction;
            DNA[2] = 130 * Rnd.NextDouble() + 20; //Food perception range;
            DNA[3] = 130 * Rnd.NextDouble() + 20; //Poison perception range;
            my_palette = new ColorPalette(Environment.CurrentDirectory + "\\AgentLife.cpl");
            my_colors = my_palette.GetColorBrushes(200);
            my_Poly = new Polygon();
            my_Poly.Points.Add(new Point(0, 0));
            my_Poly.Points.Add(new Point(-20, -5));
            my_Poly.Points.Add(new Point(-20, 5));
            my_Poly.Stroke = Brushes.Black;
            my_Poly.StrokeThickness = 1;
            my_Poly.SetValue(Canvas.LeftProperty, Location.X);
            my_Poly.SetValue(Canvas.TopProperty, Location.Y);
            my_Poly.RenderTransform = my_RT;
            my_FoodVisual = new Ellipse
            {
                Width = DNA[2],
                Height = DNA[2],
                Stroke = Brushes.Green,
                StrokeThickness = 1
            };
            my_FoodVisual.SetValue(Canvas.LeftProperty, Location.X - DNA[2] / 2);
            my_FoodVisual.SetValue(Canvas.TopProperty, Location.Y - DNA[2] / 2);
            my_PoisonVisual = new Ellipse
            {
                Width = DNA[3],
                Height = DNA[3],
                Stroke = Brushes.Red,
                StrokeThickness = 1
            };
            my_PoisonVisual.SetValue(Canvas.LeftProperty, Location.X - DNA[3] / 2);
            my_PoisonVisual.SetValue(Canvas.TopProperty, Location.Y - DNA[3] / 2);
        }

        public Agent(Agent original)
        {
            Location = new Vector(original.Location.X, original.Location.Y);
            Velocity = new Vector(Rnd.NextDouble(), Rnd.NextDouble());
            Acceleration = new Vector(0, 0);
            my_Mass = original.my_Mass;
            my_MaxSpeed = original.my_MaxSpeed;
            my_MaxForce = original.my_MaxForce;
            Health = 100;
            Age = original.Age;
            my_RT.Angle = 0;
            DNA[0] = original.DNA[0];
            DNA[1] = original.DNA[1];
            DNA[2] = original.DNA[2];
            DNA[3] = original.DNA[3];
            my_palette = new ColorPalette(Environment.CurrentDirectory + "\\AgentLife.cpl");
            my_colors = my_palette.GetColorBrushes(200);
            my_Poly = new Polygon();
            my_Poly.Points.Add(new Point(0, 0));
            my_Poly.Points.Add(new Point(-20, -5));
            my_Poly.Points.Add(new Point(-20, 5));
            my_Poly.Stroke = Brushes.Black;
            my_Poly.StrokeThickness = 1;
            my_Poly.SetValue(Canvas.LeftProperty, Location.X);
            my_Poly.SetValue(Canvas.TopProperty, Location.Y);
            my_Poly.RenderTransform = my_RT;
            my_FoodVisual = new Ellipse
            {
                Width = DNA[2],
                Height = DNA[2],
                Stroke = Brushes.Green,
                StrokeThickness = 1
            };
            my_FoodVisual.SetValue(Canvas.LeftProperty, Location.X - DNA[2] / 2);
            my_FoodVisual.SetValue(Canvas.TopProperty, Location.Y - DNA[2] / 2);
            my_PoisonVisual = new Ellipse
            {
                Width = DNA[3],
                Height = DNA[3],
                Stroke = Brushes.Red,
                StrokeThickness = 1
            };
            my_PoisonVisual.SetValue(Canvas.LeftProperty, Location.X - DNA[3] / 2);
            my_PoisonVisual.SetValue(Canvas.TopProperty, Location.Y - DNA[3] / 2);
        }

        public void Draw(Canvas c)
        {
            my_Canvas = c;
            my_Poly.SetValue(Canvas.LeftProperty, Location.X);
            my_Poly.SetValue(Canvas.TopProperty, Location.Y);
            my_FoodVisual.Width = DNA[2];
            my_FoodVisual.Height = DNA[2];
            my_FoodVisual.SetValue(Canvas.LeftProperty, Location.X - DNA[2] / 2);
            my_FoodVisual.SetValue(Canvas.TopProperty, Location.Y - DNA[2] / 2);
            my_PoisonVisual.Width = DNA[3];
            my_PoisonVisual.Height = DNA[3];
            my_PoisonVisual.SetValue(Canvas.LeftProperty, Location.X - DNA[3] / 2);
            my_PoisonVisual.SetValue(Canvas.TopProperty, Location.Y - DNA[3] / 2);
            c.Children.Add(my_Poly);
            c.Children.Add(my_FoodVisual);
            c.Children.Add(my_PoisonVisual);
        }

        //'' <summary>
        //'' Get the direction of the force towards food and poison
        //'' </summary>
        private Vector GetSteeringForce(Vector target)
        {
            Vector DesiredVelocity;
            Vector Steering;
            DesiredVelocity = target - Location;
            Steering = DesiredVelocity - Velocity;
            Steering.Normalize();
            return Steering;
        }

        //'' <summary>
        //'' Get a force away from the edge
        //'' </summary>
        private Vector GetEdgeSteeringForce()
        {
            if (my_Canvas == null) return new Vector(0, 0);
            Vector center = new Vector(my_Canvas.ActualWidth / 2, my_Canvas.ActualHeight / 2);
            Vector DesiredVelocity = new Vector(0, 0);
            Vector EdgeSteeringForce = new Vector(0, 0);
            if (Location.X < EdgeDistance)
            {
                DesiredVelocity = center - Location;
            }
            else if (Location.X > my_Canvas.ActualWidth - EdgeDistance)
            {
                DesiredVelocity = center - Location;
            }
            else if (Location.Y < EdgeDistance)
            {
                DesiredVelocity = center - Location;
            }
            else if (Location.Y > my_Canvas.ActualHeight - EdgeDistance)
            {
                DesiredVelocity = center - Location;
            }
            if (DesiredVelocity.Length > 0)
            {
                DesiredVelocity.Normalize();
                DesiredVelocity = my_MaxSpeed * DesiredVelocity;
                EdgeSteeringForce = DesiredVelocity - Velocity;
            }
            return EdgeSteeringForce;
        }

        //'' <summary>
        //'' Get the sum of all forces on the Agent
        //'' </summary>
        private Vector GetForce(Vector closestFood, Vector closestPoison)
        {
            Vector totalForce = new Vector(0, 0);
            totalForce += GetEdgeSteeringForce();
            if (closestFood.X + closestFood.Y > -1)
            {
                totalForce += DNA[0] * GetSteeringForce(closestFood);
            }
            if (closestPoison.X + closestPoison.Y > -1)
            {
                totalForce += DNA[1] * GetSteeringForce(closestPoison);
            }
            if (totalForce.Length > my_MaxForce)
            {
                totalForce.Normalize();
                totalForce = my_MaxForce * totalForce;
            }
            return totalForce;
        }

        public void Update(List<Vector> food, List<Vector> poison, int foodvalue, int poisonvalue)
        {
            double mindist;
            double dist;
            Vector closestFood;
            Vector closestPoison;
            //Adjust time dependant values
            Health -= 0.2;
            Age += 1;
            //Find closest Food in visual range
            mindist = double.MaxValue;
            closestFood = new Vector(-1, -1);
            for (int I = food.Count - 1; I >= 0; I -= 1)
            {
                dist = Math.Sqrt((Location.X - food[I].X) * (Location.X - food[I].X) + (Location.Y - food[I].Y) * (Location.Y - food[I].Y));
                if (dist < EatDistance)
                {
                    //Eat the food
                    food.Remove(food[I]);
                    Health += foodvalue;
                }
                else if (dist < DNA[2] / 2)
                {
                    if (dist < mindist)
                    {
                        mindist = dist;
                        closestFood = food[I];
                    }
                }
            }
            //Find closest Poison in visual range
            mindist = double.MaxValue;
            closestPoison = new Vector(-1, -1);
            for (int I = poison.Count - 1; I >= 0; I -= 1)
            {
                dist = Math.Sqrt((Location.X - poison[I].X) * (Location.X - poison[I].X) + (Location.Y - poison[I].Y) * (Location.Y - poison[I].Y));
                if (dist < EatDistance)
                {
                    //Eat the poison
                    poison.Remove(poison[I]);
                    Health -= poisonvalue;
                }
                else if (dist < DNA[3] / 2)
                {
                    if (dist < mindist)
                    {
                        mindist = dist;
                        closestPoison = poison[I];
                    }
                }
            }
            //Apply the forces
            Force = GetForce(closestFood, closestPoison);
            Acceleration = Force / my_Mass;
            Velocity += Acceleration;
            //Always move at maxspeed
            if (Velocity.Length > 0)
            {
                Velocity.Normalize();
                Velocity = my_MaxSpeed * Velocity;
            }
            Location += Velocity;
            //Adjust the orientation of the Agent
            double angle = 180 * Math.Atan2(Velocity.Y, Velocity.X) / Math.PI;
            my_RT.Angle = angle;
            //Adjust the color to show the Agent health
            int col = (int)Health;
            if (col > 199) col = 199;
            if (col < 0) col = 0;
            my_Poly.Fill = my_colors[col];
        }

        public void Mutate(double mutationrate, double StepSize)
        {
            double r;
            if (100 * Rnd.NextDouble() < mutationrate)
            {
                //Food attraction mutation
                r = Rnd.NextDouble();
                if (r > 0.66)
                {
                    DNA[0] = DNA[0] + StepSize;
                }
                else if (r > 0.33)
                {
                    DNA[0] = DNA[0] - StepSize;
                }
                else
                {
                    DNA[0] = Rnd.NextDouble() - 0.5;
                }
            }
            if (100 * Rnd.NextDouble() < mutationrate)
            {
                //Poison attraction mutation
                r = Rnd.NextDouble();
                if (r > 0.66)
                {
                    DNA[1] = DNA[1] + StepSize;
                }
                else if (r > 0.33)
                {
                    DNA[1] = DNA[1] - StepSize;
                }
                else
                {
                    DNA[1] = Rnd.NextDouble() - 0.5;
                }
            }
            if (100 * Rnd.NextDouble() < mutationrate)
            {
                //Food Perception Range mutation
                r = Rnd.NextDouble();
                if (r > 0.66)
                {
                    if (DNA[2] <= 140) DNA[2] = DNA[2] + 10;
                }
                else if (r > 0.33)
                {
                    if (DNA[2] >= 30) DNA[2] = DNA[2] - 10;
                }
                else
                {
                    DNA[2] = 130 * Rnd.NextDouble() + 20;
                }
            }
            if (100 * Rnd.NextDouble() < mutationrate)
            {
                //Poison Perception Range mutation
                r = Rnd.NextDouble();
                if (r > 0.66)
                {
                    if (DNA[3] <= 140) DNA[3] = DNA[3] + 10;
                }
                else if (r > 0.33)
                {
                    if (DNA[3] >= 30) DNA[3] = DNA[3] - 10;
                }
                else
                {
                    DNA[3] = 130 * Rnd.NextDouble() + 20;
                }
            }
        }

        public string Status()
        {
            string result = "";
            result += "  Age = " + Age.ToString() + "\t";
            result += "  FoodAtt = " + DNA[0].ToString("F2") + "\t";
            result += "  PoisonAtt = " + DNA[1].ToString("F2") + "\t";
            result += "  FoodRange = " + DNA[2].ToString("F2") + "\t";
            result += "  PoisonRange = " + DNA[3].ToString("F2");
            return result;
        }
    }
}
