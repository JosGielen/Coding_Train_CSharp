using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Worley_Noise
{
    class Seed
    {
        private Vector my_Pos;
        private Vector my_Speed;
        private Vector my_Acceleration;
        private Vector[] Forces;
        private Vector my_Force;
        private double[] Distances;


        public Seed(double X, double Y, Vector dir)
        {
            my_Pos = new Vector(X, Y);
            my_Speed = dir;
        }

        public Vector Pos
        {
            get { return my_Pos; }
        }

        public double X
        {
            get { return my_Pos.X; }
        }

        public double Y
        {
            get { return my_Pos.Y; }
        }

        public void Update(double TargetDistance, double maxSpeed, Vector Center, List<Seed> Seeds)
        {
            double dist;
            double totalDist = 0;
            Distances = new double[Seeds.Count];
            Forces = new Vector[Seeds.Count];
            for (int I = 0; I < Seeds.Count; I++)
            {
                dist = Math.Sqrt((Seeds[I].X - my_Pos.X) * (Seeds[I].X - my_Pos.X) + (Seeds[I].Y - my_Pos.Y) * (Seeds[I].Y - my_Pos.Y));
                Distances[I] = dist;
                totalDist += dist;
                Forces[I] = my_Pos - Seeds[I].Pos;
                if (dist > 0)
                {
                    Forces[I].Normalize();
                    if (dist < 0.1 * TargetDistance)
                    {
                        Forces[I] = 300 * Forces[I];
                    }
                    else
                    {
                        Forces[I] = (TargetDistance - dist) * Forces[I];
                    }
                }
            }
            if (totalDist > 0)
            {
                //Sort the Distances in reverse order
                double dummyDist;
                Vector dummyForce;
                for (int I = 0; I < Distances.Length; I++)
                {
                    for (int J = I + 1; J < Distances.Length; J++)
                    {
                        if (Distances[I] > Distances[J] )
                        {
                            dummyDist = Distances[I];
                            Distances[I] = Distances[J];
                            Distances[J] = dummyDist; 
                            dummyForce = Forces[I];
                            Forces[I] = Forces[J];
                            Forces[J] = dummyForce;
                        }
                    }
                }
                //Only consider the 4 closest seeds.
                for (int I = 0; I < 4; I++)
                {
                    if (Distances.Length > I)
                    {
                        if (Distances[I] > 0)
                        {
                            my_Force += Forces[I];
                        }
                    }
                }
                Vector CenterForce = Center - my_Pos;
                my_Force += 0.1 * CenterForce; 
                my_Acceleration = my_Force;
                my_Speed += my_Acceleration;
                if (my_Speed.Length > maxSpeed)
                {
                    my_Speed.Normalize();
                    my_Speed = maxSpeed * my_Speed;
                }
                my_Pos += my_Speed;
                my_Force = 0 * my_Force;
            }
        }

        public Seed Devide()
        {
            Random Rnd = new Random();
            return new Seed(my_Pos.X + 2 * Rnd.NextDouble() + 2, my_Pos.Y + 2 * Rnd.NextDouble() + 2, new Vector());
        }

        public void Edges(double W, double H)
        {
            if (my_Pos.X <= 0)
            {
                my_Pos.X = 1;
                my_Speed.X *= -1;
            }
            if (my_Pos.X >= W)
            {
                my_Pos.X = W - 1;
                my_Speed.X *= -1;
            }
            if (my_Pos.Y <= 0)
            {
                my_Pos.Y = 1;
                my_Speed.Y *= -1;
            }
            if (my_Pos.Y >= H)
            {
                my_Pos.Y = H - 1;
                my_Speed.Y *= -1;
            }
        }
    }    
}
