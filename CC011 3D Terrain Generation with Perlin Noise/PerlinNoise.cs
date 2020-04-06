
using System;

namespace _3D_Perlin_Terrain
{
    // C Sharp .Net version of Perlin Noise
    // Based on Java code by Ken Perlin
    // Ported and modified by Jos Gielen 02/03/2019

    class PerlinNoise
    {
        private static readonly Random Rnd = new Random();
        private static double XOff;
        private static double YOff;
        private static double ZOff;
        private static double WOff;
        private static readonly int[] p = new int[512];
        private static readonly int[] permutations = {151, 160, 137, 91, 90, 15,                             //Hash lookup table As defined by Ken Perlin.  
        131, 13, 201, 95, 96, 53, 194, 233, 7, 225, 140, 36, 103, 30, 69, 142, 8, 99, 37, 240, 21, 10, 23,   //This Is a randomly arranged array of all numbers 
        190, 6, 148, 247, 120, 234, 75, 0, 26, 197, 62, 94, 252, 219, 203, 117, 35, 11, 32, 57, 177, 33,     //from 0-255 inclusive.
        88, 237, 149, 56, 87, 174, 20, 125, 136, 171, 168, 68, 175, 74, 165, 71, 134, 139, 48, 27, 166,
        77, 146, 158, 231, 83, 111, 229, 122, 60, 211, 133, 230, 220, 105, 92, 41, 55, 46, 245, 40, 244,
        102, 143, 54, 65, 25, 63, 161, 1, 216, 80, 73, 209, 76, 132, 187, 208, 89, 18, 169, 200, 196,
        135, 130, 116, 188, 159, 86, 164, 100, 109, 198, 173, 186, 3, 64, 52, 217, 226, 250, 124, 123,
        5, 202, 38, 147, 118, 126, 255, 82, 85, 212, 207, 206, 59, 227, 47, 16, 58, 17, 182, 189, 28, 42,
        223, 183, 170, 213, 119, 248, 152, 2, 44, 154, 163, 70, 221, 153, 101, 155, 167, 43, 172, 9,
        129, 22, 39, 253, 19, 98, 108, 110, 79, 113, 224, 232, 178, 185, 112, 104, 218, 246, 97, 228,
        251, 34, 242, 193, 238, 210, 144, 12, 191, 179, 162, 241, 81, 51, 145, 235, 249, 14, 239, 107,
        49, 192, 214, 31, 181, 199, 106, 157, 184, 84, 204, 176, 115, 121, 50, 45, 127, 4, 150, 254,
        138, 236, 205, 93, 222, 114, 67, 29, 24, 72, 243, 141, 128, 195, 78, 66, 215, 61, 156, 180 };

        static PerlinNoise()
        {
            for (int I = 0; I < 256; I++)
            {
                p[I] = permutations[I];
                p[I + 256] = permutations[I];
            }
            Randomize();
        }

        #region "1 Dimensional Noise"

        /// <summary>
        /// Calculates a single octave Widened 1D Perlin Noise value between 0 and 1 for input value x.
        /// </summary>
        /// <param name="x">The input of the 1D Perlin Noise function.</param>
        /// <param name="factor">The number of times the widening function is applied.</param>
        public static double WideNoise(double x, int factor)
        {
            return SmoothStep(Perlin(x + XOff), 0, 1, factor);
        }

        /// <summary>
        /// Calculates a Widened 1D Perlin Noise value between 0 and 1 for input value x with given octaves and persistence.
        /// </summary>
        /// <param name="x">The input of the 1D Perlin Noise function.</param>
        /// <param name="octaves">Number of octaves used to calculate the 1D Perlin Noise.</param>
        /// <param name="persistence">Relative strength of higher octaves in the 1D Perlin Noise function.</param>
        /// <param name="factor">The number of times the widening function is applied</param>
        public static double WideNoise(double x, int octaves, double persistence, int factor)
        {
            return SmoothStep(Noise(x, octaves, persistence), 0, 1, factor);
        }

        /// <summary>
        /// Calculates a single octave 1D Perlin Noise value between 0 and 1 for input value x.
        /// </summary>
        /// <param name="x">The input of the 1D Perlin Noise function.</param>
        public static double Noise(double x)
        {
            return Perlin(x + XOff);
        }

        /// <summary>
        /// Calculates a 1D Perlin Noise value between 0 and 1 for input value x with given octaves and persistence.
        /// </summary>
        /// <param name="x">The input of the 1D Perlin Noise function.</param>
        /// <param name="octaves">Number of octaves used to calculate the 1D Perlin Noise.</param>
        /// <param name="persistence">Relative strength of higher octaves in the 1D Perlin Noise function.</param>
        public static double Noise(double x, int octaves, double persistence)
        {
            double total = 0;
            double frequency = 1;
            double amplitude = 1;
            double maxValue = 0;  //Used for normalizing result to 0.0 - 1.0
            if (octaves <= 0) { return 0.0; }
            if (octaves == 1) { return Perlin(x + XOff); }
            for (int I = 0; I < octaves; I++)
            {
                total += Perlin(x * frequency + XOff) * amplitude;
                maxValue += amplitude;
                amplitude *= persistence;
                frequency *= 2;
            }
            return total / maxValue;
        }

        /// <summary>
        /// Calculates the standard 1D Perlin Noise value between 0 and 1 for input value x.
        /// </summary>
        /// <param name="x">The x input of the 1D Perlin Noise function.</param>
        /// <returns></returns>
        public static double Perlin(double x)
        {
            int xi = (int)Math.Floor(x) & 255;
            double xf = x - Math.Floor(x);
            double u = Fade(xf);
            int g1 = p[xi];
            int g2 = p[xi + 1];
            double d1 = Grad(g1, xf);
            double d2 = Grad(g2, xf - 1);
            double x1 = Lerp(d1, d2, u);   //Interpolate X values
            return (x1 + 1) / 2;  //Return value between 0 and 1
        }

        #endregion

        #region "2 Dimensional Noise"

        /// <summary>
        /// Calculates a single octave Widened 2D Perlin Noise value between 0 and 1 for input values x,y.
        /// </summary>
        /// <param name="x">The input of the 2D Perlin Noise function.</param>
        /// <param name="y">The y input of the 2D Perlin Noise function.</param>
        /// <param name="factor">The number of times the widening function is applied.</param>
        public static double WideNoise2D(double x, double y, int factor)
        {
            return SmoothStep(Perlin2D(x + XOff , y + YOff), 0, 1, factor);
        }

        /// <summary>
        /// Calculates a widened 2D Perlin Noise value between 0 and 1 for input values x,y with given octaves and persistence.
        /// </summary>
        /// <param name="x">The x input of the 2D Perlin Noise function.</param>
        /// <param name="y">The y input of the 2D Perlin Noise function.</param>
        /// <param name="octaves">Number of octaves used to calculate the 2D Perlin Noise.</param>
        /// <param name="persistence">Relative strength of higher octaves in the 2D Perlin Noise function.</param>
        /// <param name="factor">The number of times the widening function is applied.</param>
        public static double WideNoise2D(double x, double y, int octaves, double persistence, int factor)
        {
            return SmoothStep(Noise2D(x, y, octaves, persistence), 0, 1, factor);
        }

        /// <summary>
        /// Calculates a single octave 2D Perlin Noise value between 0 and 1 for input values x,y.
        /// </summary>
        /// <param name="x">The x input of the 2D Perlin Noise function.</param>
        /// <param name="y">The y input of the 2D Perlin Noise function.</param>
        public static double Noise2D(double x, double y)
        {
            return Perlin2D(x  + XOff, y  + YOff);
        }

        /// <summary>
        /// Calculates a 2D Perlin Noise value between 0 and 1 for input values x,y with given octaves and persistence.
        /// </summary>
        /// <param name="x">The x input of the 2D Perlin Noise function.</param>
        /// <param name="y">The y input of the 2D Perlin Noise function.</param>
        /// <param name="octaves">Number of octaves used to calculate the 2D Perlin Noise.</param>
        /// <param name="persistence">Relative strength of higher octaves in the 2D Perlin Noise function.</param>
        public static double Noise2D(double x, double y, int octaves, double persistence)
        {
            double total = 0;
            double frequency = 1;
            double amplitude = 1;
            double maxValue = 0;  //Used for normalizing result to 0.0 - 1.0
            if (octaves <= 0) { return 0.0; }
            if (octaves == 1) { return Perlin2D(x + XOff, y + YOff); }
            for (int I = 0; I < octaves; I++)
            {
                total += Perlin2D(x * frequency + XOff, y * frequency + YOff) * amplitude;
                maxValue += amplitude;
                amplitude *= persistence;
                frequency *= 2;
            }
            return total / maxValue;
        }

        /// <summary>
        /// Calculates the standard 2D Perlin Noise value between 0 and 1 for input values x,y.
        /// </summary>
        /// <param name="x">The x input of the 2D Perlin Noise function.</param>
        /// <param name="y">The y input of the 2D Perlin Noise function.</param>
        /// <returns></returns>
        public static double Perlin2D(double x, double y)
        {
            int xi = (int)Math.Floor(x) & 255;
            int yi = (int)Math.Floor(y) & 255;
            int g1 = p[p[xi] + yi];
            int g2 = p[p[xi + 1] + yi];
            int g3 = p[p[xi] + yi + 1];
            int g4 = p[p[xi + 1] + yi + 1];
            double xf = x - Math.Floor(x);
            double yf = y - Math.Floor(y);
            double d1 = Grad2D(g1, xf, yf);
            double d2 = Grad2D(g2, xf - 1, yf);
            double d3 = Grad2D(g3, xf, yf - 1);
            double d4 = Grad2D(g4, xf - 1, yf - 1);
            double u = Fade(xf);
            double v = Fade(yf);
            double x1 = Lerp(d1, d2, u);   //Interpolate X at Y
            double x2 = Lerp(d3, d4, u);   //Interpolate X at Y-1
            double y1 = Lerp(x1, x2, v);   //Interpolate Y between both X interpolated values
            return (y1 + 1) / 2;  //Return value between 0 and 1
        }

        #endregion

        #region "3 Dimensional Noise"

        /// <summary>
        /// Calculates a single octave widened 3D Perlin Noise value between 0 and 1 for input values x,y,z.
        /// </summary>
        /// <param name="x">The x input of the 3D Perlin Noise function.</param>
        /// <param name="y">The y input of the 3D Perlin Noise function.</param>
        /// <param name="z">The z input of the 3D Perlin Noise function.</param>
        /// <param name="factor">The number of times the widening function is applied.</param>
        public static double WideNoise3D(double x, double y, double z, int factor)
        {
            return SmoothStep(Perlin3D(x + XOff, y + YOff, z + ZOff), 0, 1, factor);
        }

        /// <summary>
        /// Calculates a widened 3D Perlin Noise value between 0 and 1 for input values x,y,z with given octaves and persistence.
        /// </summary>
        /// <param name="x">The x input of the 3D Perlin Noise function.</param>
        /// <param name="y">The y input of the 3D Perlin Noise function.</param>
        /// <param name="z">The z input of the 3D Perlin Noise function.</param>
        /// <param name="octaves">Number of octaves used to calculate the 3D Perlin Noise.</param>
        /// <param name="persistence">Relative strength of higher octaves in the 3D Perlin Noise function.</param>
        /// <param name="factor">The number of times the widening function is applied.</param>
        public static double WideNoise3D(double x, double y, double z, int octaves, double persistence, int factor)
        {
            return SmoothStep(Noise3D(x, y, z, octaves, persistence), 0, 1, factor);
        }

        /// <summary>
        /// Calculates a single octave 3D Perlin Noise value between 0 and 1 for input values x,y,z.
        /// </summary>
        /// <param name="x">The x input of the 3D Perlin Noise function.</param>
        /// <param name="y">The y input of the 3D Perlin Noise function.</param>
        /// <param name="z">The z input of the 3D Perlin Noise function.</param>
        public static double Noise3D(double x, double y, double z)
        {
            return Perlin3D(x + XOff, y + YOff, z + ZOff);
        }

        /// <summary>
        /// Calculates a 3D Perlin Noise value between 0 and 1 for input values x,y,z with given octaves and persistence.
        /// </summary>
        /// <param name="x">The x input of the 3D Perlin Noise function.</param>
        /// <param name="y">The y input of the 3D Perlin Noise function.</param>
        /// <param name="z">The z input of the 3D Perlin Noise function.</param>
        /// <param name="octaves">Number of octaves used to calculate the 3D Perlin Noise.</param>
        /// <param name="persistence">Relative strength of higher octaves in the 3D Perlin Noise function.</param>
        public static double Noise3D(double x, double y, double z, int octaves, double persistence)
        {
            double total = 0;
            double frequency = 1;
            double amplitude = 1;
            double maxValue = 0;  //Used for normalizing result to 0.0 - 1.0
            if (octaves <= 0) { return 0.0; }
            if (octaves == 1) { return Perlin3D(x + XOff, y + YOff, z + ZOff); }
            for (int I = 0; I < octaves; I++)
            {
                total += Perlin3D(x * frequency + XOff, y * frequency + YOff, z * frequency + ZOff) * amplitude;
                maxValue += amplitude;
                amplitude *= persistence;
                frequency *= 2;
            }
            return total / maxValue;
        }

        /// <summary>
        /// Calculates the standard 3D Perlin Noise value between 0 and 1 for input values x,y,z.
        /// </summary>
        /// <param name="x">The x input of the 3D Perlin Noise function.</param>
        /// <param name="y">The y input of the 3D Perlin Noise function.</param>
        /// <param name="z">The z input of the 3D Perlin Noise function.</param>
        /// <returns></returns>
        public static double Perlin3D(double x, double y, double z)
        {
            int xi = (int)Math.Floor(x) & 255;
            int yi = (int)Math.Floor(y) & 255;
            int zi = (int)Math.Floor(z) & 255;
            int g1 = p[p[p[xi] + yi] + zi];
            int g2 = p[p[p[xi] + yi + 1] + zi];
            int g3 = p[p[p[xi] + yi] + zi + 1];
            int g4 = p[p[p[xi] + yi + 1] + zi + 1];
            int g5 = p[p[p[xi + 1] + yi] + zi];
            int g6 = p[p[p[xi + 1] + yi + 1] + zi];
            int g7 = p[p[p[xi + 1] + yi] + zi + 1];
            int g8 = p[p[p[xi + 1] + yi + 1] + zi + 1];
            double xf = x - Math.Floor(x);
            double yf = y - Math.Floor(y);
            double zf = z - Math.Floor(z);
            double d1 = Grad3D(g1, xf, yf, zf);
            double d2 = Grad3D(g2, xf, yf - 1, zf);
            double d3 = Grad3D(g3, xf, yf, zf - 1);
            double d4 = Grad3D(g4, xf, yf - 1, zf - 1);
            double d5 = Grad3D(g5, xf - 1, yf, zf);
            double d6 = Grad3D(g6, xf - 1, yf - 1, zf);
            double d7 = Grad3D(g7, xf - 1, yf, zf - 1);
            double d8 = Grad3D(g8, xf - 1, yf - 1, zf - 1);
            double u = Fade(xf);
            double v = Fade(yf);
            double w = Fade(zf);
            double X1 = Lerp(d1, d5, u);  //Interpolate X at (Y,Z)
            double X2 = Lerp(d2, d6, u);  //Interpolate X at (Y-1,Z)
            double X3 = Lerp(d3, d7, u);  //Interpolate X at (Y,Z-1)
            double X4 = Lerp(d4, d8, u);  //Interpolate X (Y-1,Z-1)
            double y1 = Lerp(X1, X2, v);  //Interpolate Y between both X interpolated values at Z
            double y2 = Lerp(X3, X4, v);  //Interpolate Y between both X interpolated values at Z-1
            double z1 = Lerp(y1, y2, w);  //Interpolate Z between both Y interpolated values.
            return (z1 + 1) / 2;  //Return value between 0 and 1
        }


        #endregion

        #region "4 Dimensional Noise"

        /// <summary>
        /// Calculates a single octave widened 4D Perlin Noise value between 0 and 1 for input values x,y,z,w.
        /// </summary>
        /// <param name="x">The x input of the 4D Perlin Noise function.</param>
        /// <param name="y">The y input of the 4D Perlin Noise function.</param>
        /// <param name="z">The z input of the 4D Perlin Noise function.</param>
        /// <param name="w">The w input of the 4D Perlin Noise function.</param>
        /// <param name="factor">The number of times the widening function is applied.</param>
        public static double WideNoise4D(double x, double y, double z, double w, int factor)
        {
            return SmoothStep(Perlin4D(x + XOff, y + YOff, z + ZOff, w + WOff), 0, 1, factor);
        }

        /// <summary>
        /// Calculates a widened 4D Perlin Noise value between 0 and 1 for input values x,y,z,w with given octaves and persistence.
        /// </summary>
        /// <param name="x">The x input of the 4D Perlin Noise function.</param>
        /// <param name="y">The y input of the 4D Perlin Noise function.</param>
        /// <param name="z">The z input of the 4D Perlin Noise function.</param>
        /// <param name="w">The w input of the 4D Perlin Noise function.</param>
        /// <param name="octaves">Number of octaves used to calculate the 4D Perlin Noise.</param>
        /// <param name="persistence">Relative strength of higher octaves in the 4D Perlin Noise function.</param>
        /// <param name="factor">The number of times the widening function is applied.</param>
        public static double WideNoise4D(double x, double y, double z, double w, int octaves, double persistence, int factor)
        {
            return SmoothStep(Noise4D(x, y, z, w, octaves, persistence), 0, 1, factor);
        }

        /// <summary>
        /// Calculates a single octave 4D Perlin Noise value between 0 and 1 for input values x,y,z,w.
        /// </summary>
        /// <param name="x">The x input of the 4D Perlin Noise function.</param>
        /// <param name="y">The y input of the 4D Perlin Noise function.</param>
        /// <param name="z">The z input of the 4D Perlin Noise function.</param>
        /// <param name="w">The w input of the 4D Perlin Noise function.</param>
        public static double Noise4D(double x, double y, double z, double w)
        {
            return Perlin4D(x + XOff, y + YOff, z + ZOff, w + WOff);
        }

        /// <summary>
        /// Calculates a 4D Perlin Noise value between 0 and 1 for input values x,y,z,w with given octaves and persistence.
        /// </summary>
        /// <param name="x">The x input of the 4D Perlin Noise function.</param>
        /// <param name="y">The y input of the 4D Perlin Noise function.</param>
        /// <param name="z">The z input of the 4D Perlin Noise function.</param>
        /// <param name="w">The w input of the 4D Perlin Noise function.</param>
        /// <param name="octaves">Number of octaves used to calculate the 4D Perlin Noise.</param>
        /// <param name="persistence">Relative strength of higher octaves in the 4D Perlin Noise function.</param>
        public static double Noise4D(double x, double y, double z, double w, int octaves, double persistence)
        {
            double total = 0;
            double frequency = 1;
            double amplitude = 1;
            double maxValue = 0;  //Used for normalizing result to 0.0 - 1.0
            if (octaves <= 0) { return 0.0; }
            if (octaves == 1) { return Perlin4D(x + XOff, y + YOff, z + ZOff, w + WOff); }
            for (int I = 0; I < octaves; I++)
            {
                total += Perlin4D(x * frequency + XOff, y * frequency + YOff, z * frequency + ZOff, w * frequency + WOff) * amplitude;
                maxValue += amplitude;
                amplitude *= persistence;
                frequency *= 2;
            }
            return total / maxValue;
        }

        /// <summary>
        /// Calculates the standard 4D Perlin Noise value between 0 and 1 for input values x,y,z,w.
        /// </summary>
        /// <param name="x">The x input of the 4D Perlin Noise function.</param>
        /// <param name="y">The y input of the 4D Perlin Noise function.</param>
        /// <param name="z">The z input of the 4D Perlin Noise function.</param>
        /// <param name="w">The w input of the 4D Perlin Noise function.</param>
        /// <returns></returns>
        public static double Perlin4D(double x, double y, double z, double w)
        {
            int xi = (int)Math.Floor(x) & 255;
            int yi = (int)Math.Floor(y) & 255;
            int zi = (int)Math.Floor(z) & 255;
            int wi = (int)Math.Floor(w) & 255;
            int g1 = p[p[p[p[xi] + yi] + zi] + wi];
            int g2 = p[p[p[p[xi] + yi + 1] + zi] + wi];
            int g3 = p[p[p[p[xi] + yi] + zi + 1] + wi];
            int g4 = p[p[p[p[xi] + yi + 1] + zi + 1] + wi];
            int g5 = p[p[p[p[xi] + yi] + zi] + wi + 1];
            int g6 = p[p[p[p[xi] + yi + 1] + zi] + wi + 1];
            int g7 = p[p[p[p[xi] + yi] + zi + 1] + wi + 1];
            int g8 = p[p[p[p[xi] + yi + 1] + zi + 1] + wi + 1];
            int g9 = p[p[p[p[xi + 1] + yi] + zi] + wi];
            int g10 = p[p[p[p[xi + 1] + yi + 1] + zi] + wi];
            int g11 = p[p[p[p[xi + 1] + yi] + zi + 1] + wi];
            int g12 = p[p[p[p[xi + 1] + yi + 1] + zi + 1] + wi];
            int g13 = p[p[p[p[xi + 1] + yi] + zi] + wi + 1];
            int g14 = p[p[p[p[xi + 1] + yi + 1] + zi] + wi + 1];
            int g15 = p[p[p[p[xi + 1] + yi] + zi + 1] + wi + 1];
            int g16 = p[p[p[p[xi + 1] + yi + 1] + zi + 1] + wi + 1];
            double xf = x - Math.Floor(x);
            double yf = y - Math.Floor(y);
            double zf = z - Math.Floor(z);
            double wf = w - Math.Floor(w);
            double d1 = Grad4D(g1, xf, yf, zf, wf);
            double d2 = Grad4D(g2, xf, yf - 1, zf, wf);
            double d3 = Grad4D(g3, xf, yf, zf - 1, wf);
            double d4 = Grad4D(g4, xf, yf - 1, zf - 1, wf);
            double d5 = Grad4D(g5, xf, yf, zf, wf - 1);
            double d6 = Grad4D(g6, xf, yf - 1, zf, wf - 1);
            double d7 = Grad4D(g7, xf, yf, zf - 1, wf - 1);
            double d8 = Grad4D(g8, xf, yf - 1, zf - 1, wf - 1);
            double d9 = Grad4D(g9, xf - 1, yf, zf, wf);
            double d10 = Grad4D(g10, xf - 1, yf - 1, zf, wf);
            double d11 = Grad4D(g11, xf - 1, yf, zf - 1, wf);
            double d12 = Grad4D(g12, xf - 1, yf - 1, zf - 1, wf);
            double d13 = Grad4D(g13, xf - 1, yf, zf, wf - 1);
            double d14 = Grad4D(g14, xf - 1, yf - 1, zf, wf - 1);
            double d15 = Grad4D(g15, xf - 1, yf, zf - 1, wf - 1);
            double d16 = Grad4D(g16, xf - 1, yf - 1, zf - 1, wf - 1);
            double ux = Fade(xf);
            double uy = Fade(yf);
            double uz = Fade(zf);
            double uw = Fade(wf);
            double X1 = Lerp(d1, d9, ux);   //Interpolate X at (Y,Z,W)
            double X2 = Lerp(d2, d10, ux);  //Interpolate X at (Y-1,Z,W)
            double X3 = Lerp(d3, d11, ux);  //Interpolate X at (Y,Z-1,W)
            double X4 = Lerp(d4, d12, ux);  //Interpolate X (Y-1,Z-1,W)
            double X5 = Lerp(d5, d13, ux);  //Interpolate X at (Y,Z,W-1)
            double X6 = Lerp(d6, d14, ux);  //Interpolate X at (Y-1,Z,W-1)
            double X7 = Lerp(d7, d15, ux);  //Interpolate X at (Y,Z-1,W-1)
            double X8 = Lerp(d8, d16, ux);  //Interpolate X (Y-1,Z-1,W-1)
            double y1 = Lerp(X1, X2, uy);   //Interpolate Y between both X interpolated values at Z,W
            double y2 = Lerp(X3, X4, uy);   //Interpolate Y between both X interpolated values at Z-1,W
            double y3 = Lerp(X5, X6, uy);   //Interpolate Y between both X interpolated values at Z,W-1
            double y4 = Lerp(X7, X8, uy);   //Interpolate Y between both X interpolated values at Z-1,W-1
            double z1 = Lerp(y1, y2, uz);   //Interpolate Z between both Y interpolated values at W.
            double z2 = Lerp(y3, y4, uz);   //Interpolate Z between both Y interpolated values at W-1.
            double w1 = Lerp(z1, z2, uw);   //Interpolate W between both Z interpolated values
            return (w1 + 1) / 2;  //Return value between 0 and 1
        }

        #endregion

        #region "Private"

        /// <summary>
        /// Linear Interpolation between values a and b at percentage 100*x.
        /// </summary>
        /// <param name="a">The start value (= 0%).</param>
        /// <param name="b">The end value (= 100%).</param>
        /// <param name="x">The interpolation factor (0 - 1).</param>
        private static double Lerp(double a, double b, double x)
        {
            return a + x * (b - a);
        }

        /// <summary>
        /// Fade function as defined by Ken Perlin.  
        /// <para>This changes values so that they will "ease" towards integral values.</para>
        /// </summary>
        /// <param name="t">value to be "eased" towards integer value.</param>
        private static double Fade(double t)
        {
            return t * t * t * (t * (t * 6 - 15) + 10);   //6t^5 - 15t^4 + 10t^3
        }


        private static double Grad(int hash, double x)
        {
            switch(hash & 1) //Take the first bit Of the hash value
            {
                case 0:
                {
                    return x;
                }
                case 1:
                {
                    return -x;
                }
                default:
                {
                    return 0; //Never happens
                }
            }
        }

        private static double Grad2D(int hash, double x, double y)
        {
            switch (hash & 3) //Take the first 2 bits Of the hash value
            {
                case 0:
                {
                    return x + y;
                }
                case 1:
                {
                    return -x + y;
                }
                case 2:
                {
                    return x - y;
                }
                case 3:
                {
                    return -x - y;
                }
                default:
                {
                    return 0; //Never happens
                }
            }
        }

        private static double Grad3D(int hash, double x, double y, double z)
        {
            switch (hash & 7) //Take the first 3 bits Of the hash value
            {
                case 0:
                {
                    return x + y;
                }
                case 1:
                {
                    return -x + y;
                }
                case 2:
                {
                    return x - y;
                }
                case 3:
                {
                    return -x - y;
                }
                case 4:
                {
                    return x + z;
                }
                case 5:
                {
                    return -x + z;
                }
                case 6:
                {
                    return x - z;
                }
                case 7:
                {
                    return -x - z;
                }
                default:
                {
                    return 0; //Never happens
                }
            }
        }

        private static double Grad4D(int hash, double x, double y, double z, double w)
        {
            switch (hash & 15) //Take the first 4 bits Of the hash value
            {
                case 0:
                {
                    return x + y;
                }
                case 1:
                {
                    return -x + y;
                }
                case 2:
                {
                    return x - y;
                }
                case 3:
                {
                    return -x - y;
                }
                case 4:
                {
                    return x + z;
                }
                case 5:
                {
                    return -x + z;
                }
                case 6:
                {
                    return x - z;
                }
                case 7:
                {
                    return -x - z;
                }
                case 8:
                {
                    return x + w;
                }
                case 9:
                {
                    return -x +w;
                }
                case 10:
                {
                    return x - w;
                }
                case 11:
                {
                    return -x - w;
                }
                case 12:
                {
                    return y + z;
                }
                case 13:
                {
                    return -y + z;
                }
                case 14:
                {
                    return y - z;
                }
                case 15:
                {
                    return -y - z;
                }
                default:
                {
                    return 0; //Never happens
                }
            }
        }

        private static void Randomize()
        {
            //Make random seed values for the Perlin Noise
            int N = DateTime.Now.Millisecond;
            if (N < 10) { N += 100; }
            for (int I = 0; I <= N; I++)
            {
                XOff = 10 * Rnd.NextDouble();
                YOff = 10 * Rnd.NextDouble();
                ZOff = 10 * Rnd.NextDouble();
                WOff = 10 * Rnd.NextDouble();
            }
        }

        private static double SmoothStep(double x, double lower, double upper, int strength)
        {
            //Make sure lower <= upper
            if (lower > upper)
            {
                double dummy;
                dummy = lower;
                lower = upper;
                upper = dummy;
            }
            //Lock x between lower and upper
            if (x < lower) { x = lower; }
            if (x > upper) { x = upper; }
            //Scale x between 0 and 1
            x = (x - lower) / (upper - lower);
            //Widen x
            for (int I = 0; I < strength; I++)
            {
                x = x * x * x * (x * (x * 6 - 15) + 10);
            }
            //Scale x back between lower and upper
            return (upper - lower) * x + lower;
        }

        #endregion 

    }
}
