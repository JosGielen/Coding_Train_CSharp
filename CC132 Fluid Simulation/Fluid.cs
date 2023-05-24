using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fluid_Simulation
{
    class Fluid
    {
        public int N;
        public double dt;
        public double diff;
        public double visc;
        public double[,] s;
        public double[,] density;
        public double[,] Vx;
        public double[,] Vy;
        public double[,] Vx0;
        public double[,] Vy0;

        public Fluid(int size, double timestep, double diffusion, double viscosity)
        {
            N = size;
            dt = timestep;
            diff = diffusion;
            visc = viscosity;
            s = new double[N, N];
            density = new double[N, N];
            Vx = new double[N, N];
            Vy = new double[N, N];
            Vx0 = new double[N, N];
            Vy0 = new double[N, N];
        }

        public void TimeStep(int iter)
        {
            Diffuse(1, Vx0, Vx, visc, dt, iter);
            Diffuse(2, Vy0, Vy, visc, dt, iter);

            Project(Vx0, Vy0, Vx, Vy, iter);

            AdVect(1, Vx, Vx0, Vx0, Vy0, dt);
            AdVect(2, Vy, Vy0, Vx0, Vy0, dt);

            Project(Vx, Vy, Vx0, Vy0, iter);

            Diffuse(0, s, density, diff, dt, iter);
            AdVect(0, density, s, Vx, Vy, dt);
        }

        public void AddDensity(int x, int y, double amount)
        {
            density[x, y] += amount;
        }

        public void FadeDensity(double amount )
        {
            for (int J = 0; J < N; J++)
            {
                for (int I = 0; I < N; I++)
                {
                    if (density[I, J] > amount)
                    {
                        density[I, J] -= amount;
                    }
                }
            }
        }

        public void AddVelocity(int x, int y, double amountX, double amountY)
        {
            Vx[x, y] += amountX;
            Vy[x, y] += amountY;
        }

        private void Diffuse(int b, double[,] Vn, double[,] Vn0, double visc, double dt, int iter)
        {
            double a = dt * visc; // * (N - 2) * (N - 2) ;
            Lin_Solve(b, Vn, Vn0, a, 1 + 6 * a, iter);
        }

        private void Lin_Solve(int b, double[,] x, double[,] x0, double a, double c, int iter)
        {
            double cRecip = 1.0 / c;
            for (int K = 0; K < iter; K++)
            {
                for (int J = 1; J < N - 1; J++)
                {
                    for (int I = 1; I < N - 1; I++)
                    {
                        x[I, J] = (x0[I, J] + a * (x[I + 1, J] + x[I - 1, J] + x[I, J + 1] + x[I, J - 1])) * cRecip;
                    }
                }
                Set_bnd(b, x);
            }
        }

        private void Project(double[,] velocX, double[,] velocY, double[,] p, double[,] div, int iter)
        {
            for (int J = 1; J < N - 1; J++)
            {
                for (int I = 1; I < N - 1; I++)
                {
                    div[I, J] = -0.5 * (velocX[I + 1, J] - velocX[I - 1, J] + velocY[I, J + 1] - velocY[I, J - 1]) / N;
                    p[I, J] = 0;
                }
            }
            Set_bnd(0, div);
            Set_bnd(0, p);
            Lin_Solve(0, p, div, 1, 6, iter);
            for (int J = 1; J < N - 1; J++)
            {
                for (int I = 1; I < N - 1; I++)
                {
                    velocX[I, J] -= 0.5 * (p[I + 1, J] - p[I - 1, J]) * N;
                    velocY[I, J] -= 0.5 * (p[I, J + 1] - p[I, J - 1]) * N;
                }
            }
            Set_bnd(1, velocX);
            Set_bnd(2, velocY);
        }

        private void AdVect(int b, double[,] d, double[,] d0, double[,] velocX, double[,] velocY, double dt)
        {
            int i0;
            int i1;
            int j0;
            int j1;
            double s0;
            double s1;
            double t0;
            double t1;
            double x;
            double y;
            double Nfloat = N;

            for (int J = 1; J < N - 1; J++)
            {
                for (int I = 1; I < N - 1; I++)
                {
                    x = I - dt * (N - 2) * velocX[I, J];
                    y = J - dt * (N - 2) * velocY[I, J];
                    if (x < 0.5) x = 0.5;
                    if (x > N + 0.5) x = N + 0.5;
                    i0 = (int)Math.Floor(x);
                    i1 = i0 + 1;
                    if (y < 0.5) y = 0.5;
                    if (y > N + 0.5) y = N + 0.5;
                    j0 = (int)Math.Floor(y);
                    j1 = j0 + 1;
                    s1 = x - i0;
                    s0 = 1.0 - s1;
                    t1 = y - j0;
                    t0 = 1.0 - t1;
                    if (i1 < N & j1 < N)
                    {
                        d[I, J] = s0 * (t0 * d0[i0, j0] + t1 * d0[i0, j1]) + s1 * (t0 * d0[i1, j0] + t1 * d0[i1, j1]);
                    }
                }
            }
            Set_bnd(b, d);
        }

        private void Set_bnd(int b, double[,] x)
        {
            //The outer cells mirror the velocity of the cell just inside it to create an impenetrable wall
            for (int I = 1; I < N - 1; I++)  //Top and bottom outer row
            {
                if (b == 2)
                {
                    x[I, 0] = -x[I, 1];
                    x[I, N - 1] = -x[I, N - 2];
                }
                else
                {
                    x[I, 0] = x[I, 1];
                    x[I, N - 1] = x[I, N - 2];
                }
            }
            for (int J = 1; J < N - 1; J++) //Left and right outer row 
            {
                if (b == 1)
                {
                    x[0, J] = -x[1, J];
                    x[N - 1, J] = -x[N - 2, J];
                }
                else
                {
                    x[0, J] = x[1, J];
                    x[N - 1, J] = x[N - 2, J];
                }
            }
            x[0, 0] = 0.5 * (x[1, 0] + x[0, 1]);
            x[0, N - 1] = 0.5 * (x[1, N - 1] + x[0, N - 2]);
            x[N - 1, 0] = 0.5 * (x[N - 2, 0] + x[N - 1, 1]);
            x[N - 1, N - 1] = 0.5 * (x[N - 1, N - 2] + x[N - 2, N - 1]);
        }
    }
}
