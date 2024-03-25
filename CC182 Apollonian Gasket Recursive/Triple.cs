using System.Numerics;

namespace Apollonian_Gasket
{
    internal class Triple
    {
        public Circle Circle1 { get; set; }
        public Circle Circle2 { get; set; }
        public Circle Circle3 { get; set; }

        public Triple (Circle c1, Circle c2, Circle c3)
        {
            Circle1 = c1;
            Circle2 = c2;
            Circle3 = c3;
        }
        
        public List<Circle> GetTangentCircles()
        {
            double k1 = Circle1.Curvature;
            double k2 = Circle2.Curvature;
            double k3 = Circle3.Curvature;
            Complex Z1 = Circle1.Center;
            Complex Z2 = Circle2.Center;
            Complex Z3 = Circle3.Center;
            List<Circle> newCircles = new List<Circle>();
            double k4_1 = k1 + k2 + k3 + 2 * Math.Sqrt(Math.Abs(k1 * k2 + k2 * k3 + k1 * k3));
            double k4_2 = k1 + k2 + k3 - 2 * Math.Sqrt(Math.Abs(k1 * k2 + k2 * k3 + k1 * k3));
            Complex sum = k1 * Z1 + k2 * Z2 + k3 * Z3;
            Complex root = k1 * k2 * Z1 * Z2 + k2 * k3 * Z2 * Z3 + k1 * k3 * Z1 * Z3;
            if (k4_1 > 0)
            {
                Complex Z4_1 = (sum + 2.0 * Complex.Sqrt(root)) / k4_1;
                Complex Z4_2 = (sum - 2.0 * Complex.Sqrt(root)) / k4_1;
                newCircles.Add(new Circle(Z4_1, k4_1));
                newCircles.Add(new Circle(Z4_2, k4_1));
            }
            if (k4_2 > 0)
            {
                Complex Z4_3 = (sum + 2.0 * Complex.Sqrt(root)) / k4_2;
                Complex Z4_4 = (sum - 2.0 * Complex.Sqrt(root)) / k4_2;
                newCircles.Add(new Circle(Z4_3, k4_2));
                newCircles.Add(new Circle(Z4_4, k4_2));
            }
            return newCircles;
        }
    }
}
