using System;

namespace Perlin_Noise
{
    // C Sharp .Net version of OpenSimplex Noise
    // Based on Java code by Kurt Spencer
    // Ported and modified by Jos Gielen 02/03/2019
    class OpenSimplexNoise
    {
        private const double STRETCH_CONSTANT_2D = -0.211324865405187;     //(1/Math.sqrt(2+1)-1)/2
        private const double SQUISH_CONSTANT_2D = 0.366025403784439;       //(Math.sqrt(2+1)-1)/2
        private const double STRETCH_CONSTANT_3D = -1.0 / 6;               //(1/Math.sqrt(3+1)-1)/3
        private const double SQUISH_CONSTANT_3D = 1.0 / 3;                 //(Math.sqrt(3+1)-1)/3
        private const double STRETCH_CONSTANT_4D = -0.138196601125011;     //(1/Math.sqrt(4+1)-1)/4
        private const double SQUISH_CONSTANT_4D = 0.309016994374947;       //(Math.sqrt(4+1)-1)/4
        private const double NORM_CONSTANT_2D = 47;
        private const double NORM_CONSTANT_3D = 103;
        private const double NORM_CONSTANT_4D = 30;
        private static readonly short[] perm;
        private static readonly short[] permGradIndex3D;

        static OpenSimplexNoise()
        {
            long seed;
            Random Rnd = new Random();
            perm = new short[256];
            permGradIndex3D = new short[256];
            short[] source = new short[256];
            for (short I = 0; I < 256; I++)
            {
                source[I] = I;
            }
            for (int I = 255; I >= 0; I--)
            {
                seed = (long)(Rnd.NextDouble() * long.MaxValue);
                int r = (int)((seed + 31) % (I + 1));
                if (r < 0) { r += I + 1; }
                perm[I] = source[r];
                permGradIndex3D[I] = (short)((perm[I] % (Gradients3D.Length / 3)) * 3);
                source[r] = source[I];
            }
        }

        #region "1 Dimensional Noise"

        /// <summary>
        /// Calculates a single octave Widened 1D OpenSimplex Noise value between 0 and 1 for input value x.
        /// </summary>
        /// <param name="x">The input of the 1D OpenSimplex Noise function.</param>
        /// <param name="factor">The number of times the widening function is applied.</param>
        public static double WideNoise(double x, int factor)
        {
            return WideNoise2D(x, 0, factor);
        }

        /// <summary>
        /// Calculates a Widened 1D OpenSimplex Noise value between 0 and 1 for input value x with given octaves and persistence.
        /// </summary>
        /// <param name="x">The input of the 1D OpenSimplex Noise function.</param>
        /// <param name="octaves">Number of octaves used to calculate the 1D OpenSimplex Noise.</param>
        /// <param name="persistence">Relative strength of higher octaves in the 1D OpenSimplex Noise function.</param>
        /// <param name="factor">The number of times the widening function is applied</param>
        public static double WideNoise(double x, int octaves, double persistence, int factor)
        {
            return WideNoise2D(x, 0, octaves, persistence, factor);
        }

        /// <summary>
        /// Calculates a single octave 1D OpenSimplex Noise value between 0 and 1 for input value x.
        /// </summary>
        /// <param name="x">The input of the 1D OpenSimplex Noise function.</param>
        public static double Noise(double x)
        {
            return Noise2D(x, 0);
        }

        /// <summary>
        /// Calculates a 1D OpenSimplex Noise value between 0 and 1 for input value x with given octaves and persistence.
        /// </summary>
        /// <param name="x">The input of the 1D OpenSimplex Noise function.</param>
        /// <param name="octaves">Number of octaves used to calculate the 1D OpenSimplex Noise.</param>
        /// <param name="persistence">Relative strength of higher octaves in the 1D OpenSimplex Noise function.</param>
        public static double Noise(double x, int octaves, double persistence)
        {
            return Noise2D(x, 0, octaves, persistence);
        }

        #endregion

        #region "2 Dimensional Noise"

        /// <summary>
        /// Calculates a single octave Widened 2D OpenSimplex Noise value between 0 and 1 for input values x,y.
        /// </summary>
        /// <param name="x">The input of the 2D OpenSimplex Noise function.</param>
        /// <param name="y">The y input of the 2D OpenSimplex Noise function.</param>
        /// <param name="factor">The number of times the widening function is applied.</param>
        public static double WideNoise2D(double x, double y, int factor)
        {
            return SmoothStep(Noise2D(x, y), 0, 1, factor);
        }

        /// <summary>
        /// Calculates a widened 2D OpenSimplex Noise value between 0 and 1 for input values x,y with given octaves and persistence.
        /// </summary>
        /// <param name="x">The x input of the 2D OpenSimplex Noise function.</param>
        /// <param name="y">The y input of the 2D OpenSimplex Noise function.</param>
        /// <param name="octaves">Number of octaves used to calculate the 2D OpenSimplex Noise.</param>
        /// <param name="persistence">Relative strength of higher octaves in the 2D OpenSimplex Noise function.</param>
        /// <param name="factor">The number of times the widening function is applied.</param>
        public static double WideNoise2D(double x, double y, int octaves, double persistence, int factor)
        {
            return SmoothStep(Noise2D(x, y, octaves, persistence), 0, 1, factor);
        }

        /// <summary>
        /// Calculates a single octave 2D OpenSimplex Noise value between 0 and 1 for input values x,y.
        /// </summary>
        /// <param name="x">The x input of the 2D OpenSimplex Noise function.</param>
        /// <param name="y">The y input of the 2D OpenSimplex Noise function.</param>
        public static double Noise2D(double x, double y)
        {
            //Place input coordinates onto grid.
            double stretchOffset = (x + y) * STRETCH_CONSTANT_2D;
            double xs = x + stretchOffset;
            double ys = y + stretchOffset;
            //Floor to get grid coordinates of rhombus (stretched square) super-cell origin.
            int xsb = FastFloor(xs);
            int ysb = FastFloor(ys);
            //Skew out to get actual coordinates of rhombus origin. We'll need these later.
            double squishOffset = (xsb + ysb) * SQUISH_CONSTANT_2D;
            double xb = xsb + squishOffset;
            double yb = ysb + squishOffset;
            //Compute grid coordinates relative to rhombus origin.
            double xins = xs - xsb;
            double yins = ys - ysb;
            //Sum those together to get a value that determines which region we're in.
            double inSum = xins + yins;
            //Positions relative to origin point.
            double dx0 = x - xb;
            double dy0 = y - yb;
            //We'll be defining these inside the next block and using them afterwards.
            double dx_ext;
            double dy_ext;
            int xsv_ext;
            int ysv_ext;
            double value = 0;
            //Contribution (1,0)
            double dx1 = dx0 - 1 - SQUISH_CONSTANT_2D;
            double dy1 = dy0 - 0 - SQUISH_CONSTANT_2D;
            double attn1 = 2 - dx1 * dx1 - dy1 * dy1;
            if (attn1 > 0)
            {
                attn1 *= attn1;
                value += attn1 * attn1 * Extrapolate(xsb + 1, ysb + 0, dx1, dy1);
            }
            ////Contribution (0,1)
            double dx2= dx0 - 0 - SQUISH_CONSTANT_2D;
            double dy2 = dy0 - 1 - SQUISH_CONSTANT_2D;
            double attn2 = 2 - dx2 * dx2 - dy2 * dy2;
            if (attn2 > 0)
            {
                attn2 *= attn2;
                value += attn2 * attn2 * Extrapolate(xsb + 0, ysb + 1, dx2, dy2);
            }
            if (inSum <= 1)  //We're inside the triangle(2 - Simplex) at(0, 0)
            {
                double zins = 1 - inSum;
                if (zins > xins | zins > yins) //(0,0) Is one of the closest two triangular vertices
                {
                    if (xins > yins)
                    {
                        xsv_ext = xsb + 1;
                        ysv_ext = ysb - 1;
                        dx_ext = dx0 - 1;
                        dy_ext = dy0 + 1;
                    }
                    else
                    {
                        xsv_ext = xsb - 1;
                        ysv_ext = ysb + 1;
                        dx_ext = dx0 + 1;
                        dy_ext = dy0 - 1;
                    }
                }
                else //(1,0) And (0,1) are the closest two vertices.
                {
                    xsv_ext = xsb + 1;
                    ysv_ext = ysb + 1;
                    dx_ext = dx0 - 1 - 2 * SQUISH_CONSTANT_2D;
                    dy_ext = dy0 - 1 - 2 * SQUISH_CONSTANT_2D;
                }
            }
            else //We're inside the triangle(2 - Simplex) at(1, 1)
            {
                double zins = 2 - inSum;
                if (zins < xins | zins < yins) //(0,0) is one of the closest two triangular vertices
                {
                    if (xins > yins)
                    {
                        xsv_ext = xsb + 2;
                        ysv_ext = ysb + 0;
                        dx_ext = dx0 - 2 - 2 * SQUISH_CONSTANT_2D;
                        dy_ext = dy0 + 0 - 2 * SQUISH_CONSTANT_2D;
                    }
                    else
                    {
                        xsv_ext = xsb + 0;
                        ysv_ext = ysb + 2;
                        dx_ext = dx0 + 0 - 2 * SQUISH_CONSTANT_2D;
                        dy_ext = dy0 - 2 - 2 * SQUISH_CONSTANT_2D;
                    }
                }
                else //(1,0) And (0,1) are the closest two vertices.
                {
                    dx_ext = dx0;
                    dy_ext = dy0;
                    xsv_ext = xsb;
                    ysv_ext = ysb;
                }
                xsb += 1;
                ysb += 1;
                dx0 = dx0 - 1 - 2 * SQUISH_CONSTANT_2D;
                dy0 = dy0 - 1 - 2 * SQUISH_CONSTANT_2D;
            }
            ////Contribution (0,0) Or (1,1)
            double attn0 = 2 - dx0 * dx0 - dy0 * dy0;
            if (attn0 > 0)
            {
                attn0 *= attn0;
                value += attn0 * attn0 * Extrapolate(xsb, ysb, dx0, dy0);
            }
            //Extra Vertex
            double attn_ext = 2 - dx_ext * dx_ext - dy_ext * dy_ext;
            if (attn_ext > 0)
            {
                attn_ext *= attn_ext;
                value += attn_ext * attn_ext * Extrapolate(xsv_ext, ysv_ext, dx_ext, dy_ext);
            }
            return (value / NORM_CONSTANT_2D + 1) / 2;
        }

        /// <summary>
        /// Calculates a 2D OpenSimplex Noise value between 0 and 1 for input values x,y with given octaves and persistence.
        /// </summary>
        /// <param name="x">The x input of the 2D OpenSimplex Noise function.</param>
        /// <param name="y">The y input of the 2D OpenSimplex Noise function.</param>
        /// <param name="octaves">Number of octaves used to calculate the 2D OpenSimplex Noise.</param>
        /// <param name="persistence">Relative strength of higher octaves in the 2D OpenSimplex Noise function.</param>
        public static double Noise2D(double x, double y, int octaves, double persistence)
        {
            double total = 0;
            double frequency = 1;
            double amplitude = 1;
            double maxValue = 0;  //Used for normalizing result to 0.0 - 1.0
            if (octaves <= 0) { return 0.0; }
            if (octaves == 1) { return Noise2D(x, y); }
            for (int I = 0; I < octaves; I++)
            {
                total += Noise2D(x * frequency, y * frequency) * amplitude;
                maxValue += amplitude;
                amplitude *= persistence;
                frequency *= 2;
            }
            return total / maxValue;
        }

        #endregion

        #region "3 Dimensional Noise"

        /// <summary>
        /// Calculates a single octave widened 3D OpenSimplex Noise value between 0 and 1 for input values x,y,z.
        /// </summary>
        /// <param name="x">The x input of the 3D OpenSimplex Noise function.</param>
        /// <param name="y">The y input of the 3D OpenSimplex Noise function.</param>
        /// <param name="z">The z input of the 3D OpenSimplex Noise function.</param>
        /// <param name="factor">The number of times the widening function is applied.</param>
        public static double WideNoise3D(double x, double y, double z, int factor)
        {
            return SmoothStep(Noise3D(x, y, z), 0, 1, factor);
        }

        /// <summary>
        /// Calculates a widened 3D OpenSimplex Noise value between 0 and 1 for input values x,y,z with given octaves and persistence.
        /// </summary>
        /// <param name="x">The x input of the 3D OpenSimplex Noise function.</param>
        /// <param name="y">The y input of the 3D OpenSimplex Noise function.</param>
        /// <param name="z">The z input of the 3D OpenSimplex Noise function.</param>
        /// <param name="octaves">Number of octaves used to calculate the 3D OpenSimplex Noise.</param>
        /// <param name="persistence">Relative strength of higher octaves in the 3D OpenSimplex Noise function.</param>
        /// <param name="factor">The number of times the widening function is applied.</param>
        public static double WideNoise3D(double x, double y, double z, int octaves, double persistence, int factor)
        {
            return SmoothStep(Noise3D(x, y, z, octaves, persistence), 0, 1, factor);
        }

        /// <summary>
        /// Calculates a single octave 3D OpenSimplex Noise value between 0 and 1 for input values x,y,z.
        /// </summary>
        /// <param name="x">The x input of the 3D OpenSimplex Noise function.</param>
        /// <param name="y">The y input of the 3D OpenSimplex Noise function.</param>
        /// <param name="z">The z input of the 3D OpenSimplex Noise function.</param>
        public static double Noise3D(double x, double y, double z)
        {
            //Place input coordinates on simplectic honeycomb.
            double stretchOffset = (x + y + z) * STRETCH_CONSTANT_3D;
            double xs = x + stretchOffset;
            double ys = y + stretchOffset;
            double zs = z + stretchOffset;
            //Floor to get simplectic honeycomb coordinates of rhombohedron (stretched cube) super-cell origin.
            int xsb = FastFloor(xs);
            int ysb = FastFloor(ys);
            int zsb = FastFloor(zs);
            //Skew out to get actual coordinates of rhombohedron origin. We'll need these later.
            double squishOffset = (xsb + ysb + zsb) * SQUISH_CONSTANT_3D;
            double xb = xsb + squishOffset;
            double yb = ysb + squishOffset;
            double zb = zsb + squishOffset;
            //Compute simplectic honeycomb coordinates relative to rhombohedral origin.
            double xins = xs - xsb;
            double yins = ys - ysb;
            double zins = zs - zsb;
            //Sum those together to get a value that determines which region we're in.
            double inSum = xins + yins + zins;
            //Positions relative to origin point.
            double dx0 = x - xb;
            double dy0 = y - yb;
            double dz0 = z - zb;
            //We'll be defining these inside the next block and using them afterwards.
            double dx_ext0, dy_ext0, dz_ext0;
            double dx_ext1, dy_ext1, dz_ext1;
            int xsv_ext0, ysv_ext0, zsv_ext0;
            int xsv_ext1, ysv_ext1, zsv_ext1;

            double value = 0;
            if (inSum <= 1)
            {  //We're inside the tetrahedron(3 - Simplex) at(0, 0, 0)
               //Determine which two of (0,0,1), (0,1,0), (1,0,0) are closest.
                byte aPoint = 0x1;
                double aScore = xins;
                byte bPoint = 0x2;
                double bScore = yins;
                if (aScore >= bScore & zins > bScore)
                {
                    bScore = zins;
                    bPoint = 0x4;
                }
                else if (aScore < bScore & zins > aScore)
                {
                    aScore = zins;
                    aPoint = 0x4;
                }
                //Now we determine the two lattice points Not part of the tetrahedron that may contribute.
                //This depends on the closest two tetrahedral vertices, including (0,0,0)
                double wins = 1 - inSum;
                if (wins > aScore | wins > bScore) //(0,0,0) Is one of the closest two tetrahedral vertices.
                {
                    byte c;
                    //Our other closest vertex Is the closest out of a And b.
                    if (bScore > aScore)
                    {
                        c = bPoint;
                    }
                    else
                    {
                        c = aPoint;
                    }
                    if ((c & 0x1) == 0)
                    {
                        xsv_ext0 = xsb - 1;
                        xsv_ext1 = xsb;
                        dx_ext0 = dx0 + 1;
                        dx_ext1 = dx0;
                    }
                    else
                    {
                        xsv_ext0 = xsb + 1;
                        dx_ext0 = dx0 - 1;
                        xsv_ext1 = xsb + 1;
                        dx_ext1 = dx0 - 1;
                    }
                    if ((c & 0x2) == 0)
                    {
                        ysv_ext0 = ysb;
                        dy_ext0 = dy0;
                        ysv_ext1 = ysb;
                        dy_ext1 = dy0;
                        if ((c & 0x1) == 0)
                        {
                            ysv_ext1 -= 1;
                            dy_ext1 += 1;
                        }
                        else
                        {
                            ysv_ext0 -= 1;
                            dy_ext0 += 1;
                        }
                    }
                    else
                    {
                        ysv_ext0 = ysb + 1;
                        dy_ext0 = dy0 - 1;
                        ysv_ext1 = ysb + 1;
                        dy_ext1 = dy0 - 1;
                    }
                    if ((c & 0x4) == 0)
                    {
                        zsv_ext0 = zsb;
                        zsv_ext1 = zsb - 1;
                        dz_ext0 = dz0;
                        dz_ext1 = dz0 + 1;
                    }
                    else
                    {
                        zsv_ext0 = zsb + 1;
                        dz_ext0 = dz0 - 1;
                        zsv_ext1 = zsb + 1;
                        dz_ext1 = dz0 - 1;
                    }
                }
                else //(0,0,0) Is Not one of the closest two tetrahedral vertices.
                {
                    byte c = (byte)(aPoint | bPoint); //Our two extra vertices are determined by the closest two.
                    if ((c & 0x1) == 0)
                    {
                        xsv_ext0 = xsb;
                        xsv_ext1 = xsb - 1;
                        dx_ext0 = dx0 - 2 * SQUISH_CONSTANT_3D;
                        dx_ext1 = dx0 + 1 - SQUISH_CONSTANT_3D;
                    }
                    else
                    {
                        xsv_ext0 = xsb + 1;
                        xsv_ext1 = xsb + 1;
                        dx_ext0 = dx0 - 1 - 2 * SQUISH_CONSTANT_3D;
                        dx_ext1 = dx0 - 1 - SQUISH_CONSTANT_3D;
                    }
                    if ((c & 0x2) == 0)
                    {
                        ysv_ext0 = ysb;
                        ysv_ext1 = ysb - 1;
                        dy_ext0 = dy0 - 2 * SQUISH_CONSTANT_3D;
                        dy_ext1 = dy0 + 1 - SQUISH_CONSTANT_3D;
                    }
                    else
                    {
                        ysv_ext0 = ysb + 1;
                        ysv_ext1 = ysb + 1;
                        dy_ext0 = dy0 - 1 - 2 * SQUISH_CONSTANT_3D;
                        dy_ext1 = dy0 - 1 - SQUISH_CONSTANT_3D;
                    }
                    if ((c & 0x4) == 0)
                    {
                        zsv_ext0 = zsb;
                        zsv_ext1 = zsb - 1;
                        dz_ext0 = dz0 - 2 * SQUISH_CONSTANT_3D;
                        dz_ext1 = dz0 + 1 - SQUISH_CONSTANT_3D;
                    }
                    else
                    {
                        zsv_ext0 = zsb + 1;
                        zsv_ext1 = zsb + 1;
                        dz_ext0 = dz0 - 1 - 2 * SQUISH_CONSTANT_3D;
                        dz_ext1 = dz0 - 1 - SQUISH_CONSTANT_3D;
                    }
                }
                //Contribution (0,0,0)
                double attn0 = 2 - dx0 * dx0 - dy0 * dy0 - dz0 * dz0;
                if (attn0 > 0)
                {
                    attn0 *= attn0;
                    value += attn0 * attn0 * Extrapolate(xsb + 0, ysb + 0, zsb + 0, dx0, dy0, dz0);
                }
                //Contribution (1,0,0)
                double dx1 = dx0 - 1 - SQUISH_CONSTANT_3D;
                double dy1 = dy0 - 0 - SQUISH_CONSTANT_3D;
                double dz1 = dz0 - 0 - SQUISH_CONSTANT_3D;
                double attn1 = 2 - dx1 * dx1 - dy1 * dy1 - dz1 * dz1;
                if (attn1 > 0)
                {
                    attn1 *= attn1;
                    value += attn1 * attn1 * Extrapolate(xsb + 1, ysb + 0, zsb + 0, dx1, dy1, dz1);
                }
                //Contribution (0,1,0)
                double dx2 = dx0 - 0 - SQUISH_CONSTANT_3D;
                double dy2 = dy0 - 1 - SQUISH_CONSTANT_3D;
                double dz2 = dz1;
                double attn2 = 2 - dx2 * dx2 - dy2 * dy2 - dz2 * dz2;
                if (attn2 > 0)
                {
                    attn2 *= attn2;
                    value += attn2 * attn2 * Extrapolate(xsb + 0, ysb + 1, zsb + 0, dx2, dy2, dz2);
                }
                //Contribution (0,0,1)
                double dx3 = dx2;
                double dy3 = dy1;
                double dz3 = dz0 - 1 - SQUISH_CONSTANT_3D;
                double attn3 = 2 - dx3 * dx3 - dy3 * dy3 - dz3 * dz3;
                if (attn3 > 0)
                {
                    attn3 *= attn3;
                    value += attn3 * attn3 * Extrapolate(xsb + 0, ysb + 0, zsb + 1, dx3, dy3, dz3);
                }
            }
            else if (inSum >= 2) //We're inside the tetrahedron(3 - Simplex) at(1, 1, 1)
            {
                //Determine which two tetrahedral vertices are the closest, out of (1,1,0), (1,0,1), (0,1,1) but not (1,1,1).
                byte aPoint = 0x6;
                double aScore = xins;
                byte bPoint = 0x5;
                double bScore = yins;
                if (aScore <= bScore & zins < bScore)
                {
                    bScore = zins;
                    bPoint = 0x3;
                }
                else if (aScore > bScore & zins < aScore)
                {
                    aScore = zins;
                    aPoint = 0x3;
                }
                //Now we determine the two lattice points not part of the tetrahedron that may contribute.
                //This depends on the closest two tetrahedral vertices, including (1,1,1)
                double wins = 3 - inSum;
                if (wins < aScore | wins < bScore) //(1,1,1) is one of the closest two tetrahedral vertices.
                {
                    byte c;
                    if (bScore < aScore) //Our other closest vertex is the closest out of a and b.
                    {
                        c = bPoint;
                    }
                    else
                    {
                        c = aPoint;
                    }
                    if ((c & 0x1) != 0)
                    {
                        xsv_ext0 = xsb + 2;
                        xsv_ext1 = xsb + 1;
                        dx_ext0 = dx0 - 2 - 3 * SQUISH_CONSTANT_3D;
                        dx_ext1 = dx0 - 1 - 3 * SQUISH_CONSTANT_3D;
                    }
                    else
                    {
                        xsv_ext0 = xsb;
                        dx_ext0 = dx0 - 3 * SQUISH_CONSTANT_3D;
                        xsv_ext1 = xsb;
                        dx_ext1 = dx0 - 3 * SQUISH_CONSTANT_3D;
                    }
                    if ((c & 0x2) != 0)
                    {
                        ysv_ext0 = ysb + 1;
                        dy_ext0 = dy0 - 1 - 3 * SQUISH_CONSTANT_3D;
                        ysv_ext1 = ysb + 1;
                        dy_ext1 = dy0 - 1 - 3 * SQUISH_CONSTANT_3D;
                        if ((c & 0x1) != 0)
                        {
                            ysv_ext1 += 1;
                            dy_ext1 -= 1;
                        }
                        else
                        {
                            ysv_ext0 += 1;
                            dy_ext0 -= 1;
                        }
                    }
                    else
                    {
                        ysv_ext0 = ysb;
                        dy_ext0 = dy0 - 3 * SQUISH_CONSTANT_3D;
                        ysv_ext1 = ysb;
                        dy_ext1 = dy0 - 3 * SQUISH_CONSTANT_3D;
                    }
                    if ((c & 0x4) != 0)
                    {
                        zsv_ext0 = zsb + 1;
                        zsv_ext1 = zsb + 2;
                        dz_ext0 = dz0 - 1 - 3 * SQUISH_CONSTANT_3D;
                        dz_ext1 = dz0 - 2 - 3 * SQUISH_CONSTANT_3D;
                    }
                    else
                    {
                        zsv_ext0 = zsb;
                        dz_ext0 = dz0 - 3 * SQUISH_CONSTANT_3D;
                        zsv_ext1 = zsb;
                        dz_ext1 = dz0 - 3 * SQUISH_CONSTANT_3D;
                    }
                }
                else //(1,1,1) is not one of the closest two tetrahedral vertices.
                {
                    byte c = (byte)(aPoint & bPoint); //Our two extra vertices are determined by the closest two.
                    if ((c & 0x1) != 0)
                    {
                        xsv_ext0 = xsb + 1;
                        xsv_ext1 = xsb + 2;
                        dx_ext0 = dx0 - 1 - SQUISH_CONSTANT_3D;
                        dx_ext1 = dx0 - 2 - 2 * SQUISH_CONSTANT_3D;
                    }
                    else
                    {
                        xsv_ext0 = xsb;
                        xsv_ext1 = xsb;
                        dx_ext0 = dx0 - SQUISH_CONSTANT_3D;
                        dx_ext1 = dx0 - 2 * SQUISH_CONSTANT_3D;
                    }
                    if ((c & 0x2) != 0)
                    {
                        ysv_ext0 = ysb + 1;
                        ysv_ext1 = ysb + 2;
                        dy_ext0 = dy0 - 1 - SQUISH_CONSTANT_3D;
                        dy_ext1 = dy0 - 2 - 2 * SQUISH_CONSTANT_3D;
                    }
                    else
                    {
                        ysv_ext0 = ysb;
                        ysv_ext1 = ysb;
                        dy_ext0 = dy0 - SQUISH_CONSTANT_3D;
                        dy_ext1 = dy0 - 2 * SQUISH_CONSTANT_3D;
                    }
                    if ((c & 0x4) != 0)
                    {
                        zsv_ext0 = zsb + 1;
                        zsv_ext1 = zsb + 2;
                        dz_ext0 = dz0 - 1 - SQUISH_CONSTANT_3D;
                        dz_ext1 = dz0 - 2 - 2 * SQUISH_CONSTANT_3D;
                    }
                    else
                    {
                        zsv_ext0 = zsb;
                        zsv_ext1 = zsb;
                        dz_ext0 = dz0 - SQUISH_CONSTANT_3D;
                        dz_ext1 = dz0 - 2 * SQUISH_CONSTANT_3D;
                    }
                }
                //Contribution (1,1,0)
                double dx3 = dx0 - 1 - 2 * SQUISH_CONSTANT_3D;
                double dy3 = dy0 - 1 - 2 * SQUISH_CONSTANT_3D;
                double dz3 = dz0 - 0 - 2 * SQUISH_CONSTANT_3D;
                double attn3 = 2 - dx3 * dx3 - dy3 * dy3 - dz3 * dz3;
                if (attn3 > 0)
                {
                    attn3 *= attn3;
                    value += attn3 * attn3 * Extrapolate(xsb + 1, ysb + 1, zsb + 0, dx3, dy3, dz3);
                }
                //Contribution (1,0,1)
                double dx2 = dx3;
                double dy2 = dy0 - 0 - 2 * SQUISH_CONSTANT_3D;
                double dz2 = dz0 - 1 - 2 * SQUISH_CONSTANT_3D;
                double attn2 = 2 - dx2 * dx2 - dy2 * dy2 - dz2 * dz2;
                if (attn2 > 0)
                {
                    attn2 *= attn2;
                    value += attn2 * attn2 * Extrapolate(xsb + 1, ysb + 0, zsb + 1, dx2, dy2, dz2);
                }
                //Contribution (0,1,1)
                double dx1 = dx0 - 0 - 2 * SQUISH_CONSTANT_3D;
                double dy1 = dy3;
                double dz1 = dz2;
                double attn1 = 2 - dx1 * dx1 - dy1 * dy1 - dz1 * dz1;
                if (attn1 > 0)
                {
                    attn1 *= attn1;
                    value += attn1 * attn1 * Extrapolate(xsb + 0, ysb + 1, zsb + 1, dx1, dy1, dz1);
                }
                //Contribution (1,1,1)
                dx0 = dx0 - 1 - 3 * SQUISH_CONSTANT_3D;
                dy0 = dy0 - 1 - 3 * SQUISH_CONSTANT_3D;
                dz0 = dz0 - 1 - 3 * SQUISH_CONSTANT_3D;
                double attn0 = 2 - dx0 * dx0 - dy0 * dy0 - dz0 * dz0;
                if (attn0 > 0)
                {
                    attn0 *= attn0;
                    value += attn0 * attn0 * Extrapolate(xsb + 1, ysb + 1, zsb + 1, dx0, dy0, dz0);
                }
            }
            else //We're inside the octahedron(Rectified 3 - Simplex) in between.
            {
                double aScore;
                byte aPoint;
                bool aIsFurtherSide;
                double bScore;
                byte bPoint;
                bool bIsFurtherSide;
                //Decide between point (0,0,1) and (1,1,0) as closest
                double p1 = xins + yins;
                if (p1 > 1)
                {
                    aScore = p1 - 1;
                    aPoint = 0x3;
                    aIsFurtherSide = true;
                }
                else
                {
                    aScore = 1 - p1;
                    aPoint = 0x4;
                    aIsFurtherSide = false;
                }
                //Decide between point (0,1,0) and (1,0,1) as closest
                double p2 = xins + zins;
                if (p2 > 1)
                {
                    bScore = p2 - 1;
                    bPoint = 0x5;
                    bIsFurtherSide = true;
                }
                else
                {
                    bScore = 1 - p2;
                    bPoint = 0x2;
                    bIsFurtherSide = false;
                }
                //The closest out of the two (1,0,0) and (0,1,1) will replace the furthest out of the two decided above, if closer.
                double p3 = yins + zins;
                if (p3 > 1)
                {
                    double score = p3 - 1;
                    if (aScore <= bScore & aScore < score)
                    {
                        aScore = score;
                        aPoint = 0x6;
                        aIsFurtherSide = true;
                    }
                    else if (aScore > bScore & bScore < score)
                    {
                        bScore = score;
                        bPoint = 0x6;
                        bIsFurtherSide = true;
                    }
                }
                else
                {
                    double score = 1 - p3;
                    if (aScore <= bScore & aScore < score)
                    {
                        aScore = score;
                        aPoint = 0x1;
                        aIsFurtherSide = false;
                    }
                    else if (aScore > bScore & bScore < score)
                    {
                        bScore = score;
                        bPoint = 0x1;
                        bIsFurtherSide = false;
                    }
                }
                //Where each of the two closest points are, determines how the extra two vertices are calculated.
                if (aIsFurtherSide == bIsFurtherSide)
                {
                    if (aIsFurtherSide)
                    {
                        //Both closest points on (1,1,1) side
                        //One of the two extra points is (1,1,1)
                        dx_ext0 = dx0 - 1 - 3 * SQUISH_CONSTANT_3D;
                        dy_ext0 = dy0 - 1 - 3 * SQUISH_CONSTANT_3D;
                        dz_ext0 = dz0 - 1 - 3 * SQUISH_CONSTANT_3D;
                        xsv_ext0 = xsb + 1;
                        ysv_ext0 = ysb + 1;
                        zsv_ext0 = zsb + 1;
                        //Other extra point is based on the shared axis.
                        byte c = (byte)(aPoint & bPoint);
                        if ((c & 0x1) != 0)
                        {
                            dx_ext1 = dx0 - 2 - 2 * SQUISH_CONSTANT_3D;
                            dy_ext1 = dy0 - 2 * SQUISH_CONSTANT_3D;
                            dz_ext1 = dz0 - 2 * SQUISH_CONSTANT_3D;
                            xsv_ext1 = xsb + 2;
                            ysv_ext1 = ysb;
                            zsv_ext1 = zsb;
                        }
                        else if ((c & 0x2) != 0)
                        {
                            dx_ext1 = dx0 - 2 * SQUISH_CONSTANT_3D;
                            dy_ext1 = dy0 - 2 - 2 * SQUISH_CONSTANT_3D;
                            dz_ext1 = dz0 - 2 * SQUISH_CONSTANT_3D;
                            xsv_ext1 = xsb;
                            ysv_ext1 = ysb + 2;
                            zsv_ext1 = zsb;
                        }
                        else
                        {
                            dx_ext1 = dx0 - 2 * SQUISH_CONSTANT_3D;
                            dy_ext1 = dy0 - 2 * SQUISH_CONSTANT_3D;
                            dz_ext1 = dz0 - 2 - 2 * SQUISH_CONSTANT_3D;
                            xsv_ext1 = xsb;
                            ysv_ext1 = ysb;
                            zsv_ext1 = zsb + 2;
                        }
                    }
                    else
                    {
                        //Both closest points on (0,0,0) side
                        //One of the two extra points is (0,0,0)
                        dx_ext0 = dx0;
                        dy_ext0 = dy0;
                        dz_ext0 = dz0;
                        xsv_ext0 = xsb;
                        ysv_ext0 = ysb;
                        zsv_ext0 = zsb;
                        //Other extra point is based on the omitted axis.
                        byte c = (byte)(aPoint | bPoint);
                        if ((c & 0x1) == 0)
                        {
                            dx_ext1 = dx0 + 1 - SQUISH_CONSTANT_3D;
                            dy_ext1 = dy0 - 1 - SQUISH_CONSTANT_3D;
                            dz_ext1 = dz0 - 1 - SQUISH_CONSTANT_3D;
                            xsv_ext1 = xsb - 1;
                            ysv_ext1 = ysb + 1;
                            zsv_ext1 = zsb + 1;
                        }
                        else if ((c & 0x2) == 0)
                        {
                            dx_ext1 = dx0 - 1 - SQUISH_CONSTANT_3D;
                            dy_ext1 = dy0 + 1 - SQUISH_CONSTANT_3D;
                            dz_ext1 = dz0 - 1 - SQUISH_CONSTANT_3D;
                            xsv_ext1 = xsb + 1;
                            ysv_ext1 = ysb - 1;
                            zsv_ext1 = zsb + 1;
                        }
                        else
                        {
                            dx_ext1 = dx0 - 1 - SQUISH_CONSTANT_3D;
                            dy_ext1 = dy0 - 1 - SQUISH_CONSTANT_3D;
                            dz_ext1 = dz0 + 1 - SQUISH_CONSTANT_3D;
                            xsv_ext1 = xsb + 1;
                            ysv_ext1 = ysb + 1;
                            zsv_ext1 = zsb - 1;
                        }
                    }
                }
                else //One point on (0,0,0) side, one point on (1,1,1) side
                {
                    byte c1, c2;
                    if (aIsFurtherSide)
                    {
                        c1 = aPoint;
                        c2 = bPoint;
                    }
                    else
                    {
                        c1 = bPoint;
                        c2 = aPoint;
                    }
                    //One contribution is a permutation of (1,1,-1)
                    if ((c1 & 0x1) == 0)
                    {
                        dx_ext0 = dx0 + 1 - SQUISH_CONSTANT_3D;
                        dy_ext0 = dy0 - 1 - SQUISH_CONSTANT_3D;
                        dz_ext0 = dz0 - 1 - SQUISH_CONSTANT_3D;
                        xsv_ext0 = xsb - 1;
                        ysv_ext0 = ysb + 1;
                        zsv_ext0 = zsb + 1;
                    }
                    else if ((c1 & 0x2) == 0)
                    {
                        dx_ext0 = dx0 - 1 - SQUISH_CONSTANT_3D;
                        dy_ext0 = dy0 + 1 - SQUISH_CONSTANT_3D;
                        dz_ext0 = dz0 - 1 - SQUISH_CONSTANT_3D;
                        xsv_ext0 = xsb + 1;
                        ysv_ext0 = ysb - 1;
                        zsv_ext0 = zsb + 1;
                    }
                    else
                    {
                        dx_ext0 = dx0 - 1 - SQUISH_CONSTANT_3D;
                        dy_ext0 = dy0 - 1 - SQUISH_CONSTANT_3D;
                        dz_ext0 = dz0 + 1 - SQUISH_CONSTANT_3D;
                        xsv_ext0 = xsb + 1;
                        ysv_ext0 = ysb + 1;
                        zsv_ext0 = zsb - 1;
                    }
                    //One contribution is a permutation of (0,0,2)
                    dx_ext1 = dx0 - 2 * SQUISH_CONSTANT_3D;
                    dy_ext1 = dy0 - 2 * SQUISH_CONSTANT_3D;
                    dz_ext1 = dz0 - 2 * SQUISH_CONSTANT_3D;
                    xsv_ext1 = xsb;
                    ysv_ext1 = ysb;
                    zsv_ext1 = zsb;
                    if ((c2 & 0x1) != 0)
                    {
                        dx_ext1 -= 2;
                        xsv_ext1 += 2;
                    }
                    else if ((c2 & 0x2) != 0)
                    {
                        dy_ext1 -= 2;
                        ysv_ext1 += 2;
                    }
                    else
                    {
                        dz_ext1 -= 2;
                        zsv_ext1 += 2;
                    }
                }
                //Contribution (1,0,0)
                double dx1 = dx0 - 1 - SQUISH_CONSTANT_3D;
                double dy1 = dy0 - 0 - SQUISH_CONSTANT_3D;
                double dz1 = dz0 - 0 - SQUISH_CONSTANT_3D;
                double attn1 = 2 - dx1 * dx1 - dy1 * dy1 - dz1 * dz1;
                if (attn1 > 0)
                {
                    attn1 *= attn1;
                    value += attn1 * attn1 * Extrapolate(xsb + 1, ysb + 0, zsb + 0, dx1, dy1, dz1);
                }
                //Contribution (0,1,0)
                double dx2 = dx0 - 0 - SQUISH_CONSTANT_3D;
                double dy2 = dy0 - 1 - SQUISH_CONSTANT_3D;
                double dz2 = dz1;
                double attn2 = 2 - dx2 * dx2 - dy2 * dy2 - dz2 * dz2;
                if (attn2 > 0)
                {
                    attn2 *= attn2;
                    value += attn2 * attn2 * Extrapolate(xsb + 0, ysb + 1, zsb + 0, dx2, dy2, dz2);
                }
                //Contribution (0,0,1)
                double dx3 = dx2;
                double dy3 = dy1;
                double dz3 = dz0 - 1 - SQUISH_CONSTANT_3D;
                double attn3 = 2 - dx3 * dx3 - dy3 * dy3 - dz3 * dz3;
                if (attn3 > 0)
                {
                    attn3 *= attn3;
                    value += attn3 * attn3 * Extrapolate(xsb + 0, ysb + 0, zsb + 1, dx3, dy3, dz3);
                }
                //Contribution (1,1,0)
                double dx4 = dx0 - 1 - 2 * SQUISH_CONSTANT_3D;
                double dy4 = dy0 - 1 - 2 * SQUISH_CONSTANT_3D;
                double dz4 = dz0 - 0 - 2 * SQUISH_CONSTANT_3D;
                double attn4 = 2 - dx4 * dx4 - dy4 * dy4 - dz4 * dz4;
                if (attn4 > 0)
                {
                    attn4 *= attn4;
                    value += attn4 * attn4 * Extrapolate(xsb + 1, ysb + 1, zsb + 0, dx4, dy4, dz4);
                }
                //Contribution (1,0,1)
                double dx5 = dx4;
                double dy5 = dy0 - 0 - 2 * SQUISH_CONSTANT_3D;
                double dz5 = dz0 - 1 - 2 * SQUISH_CONSTANT_3D;
                double attn5 = 2 - dx5 * dx5 - dy5 * dy5 - dz5 * dz5;
                if (attn5 > 0)
                {
                    attn5 *= attn5;
                    value += attn5 * attn5 * Extrapolate(xsb + 1, ysb + 0, zsb + 1, dx5, dy5, dz5);
                }
                //Contribution (0,1,1)
                double dx6 = dx0 - 0 - 2 * SQUISH_CONSTANT_3D;
                double dy6 = dy4;
                double dz6 = dz5;
                double attn6 = 2 - dx6 * dx6 - dy6 * dy6 - dz6 * dz6;
                if (attn6 > 0)
                {
                    attn6 *= attn6;
                    value += attn6 * attn6 * Extrapolate(xsb + 0, ysb + 1, zsb + 1, dx6, dy6, dz6);
                }
            }
            //First extra vertex
            double attn_ext0 = 2 - dx_ext0 * dx_ext0 - dy_ext0 * dy_ext0 - dz_ext0 * dz_ext0;
            if(attn_ext0 > 0)
            {
                attn_ext0 *= attn_ext0;
                value += attn_ext0 * attn_ext0 * Extrapolate(xsv_ext0, ysv_ext0, zsv_ext0, dx_ext0, dy_ext0, dz_ext0);
            }
            //Second extra vertex
            double attn_ext1 = 2 - dx_ext1 * dx_ext1 - dy_ext1 * dy_ext1 - dz_ext1 * dz_ext1;
            if(attn_ext1 > 0)
            {
                attn_ext1 *= attn_ext1;
                value += attn_ext1 * attn_ext1 * Extrapolate(xsv_ext1, ysv_ext1, zsv_ext1, dx_ext1, dy_ext1, dz_ext1);
            }
            return (value / NORM_CONSTANT_3D + 1) / 2;
        }

        /// <summary>
        /// Calculates a 3D OpenSimplex Noise value between 0 and 1 for input values x,y,z with given octaves and persistence.
        /// </summary>
        /// <param name="x">The x input of the 3D OpenSimplex Noise function.</param>
        /// <param name="y">The y input of the 3D OpenSimplex Noise function.</param>
        /// <param name="z">The z input of the 3D OpenSimplex Noise function.</param>
        /// <param name="octaves">Number of octaves used to calculate the 3D OpenSimplex Noise.</param>
        /// <param name="persistence">Relative strength of higher octaves in the 3D OpenSimplex Noise function.</param>
        public static double Noise3D(double x, double y, double z, int octaves, double persistence)
        {
            double total = 0;
            double frequency = 1;
            double amplitude = 1;
            double maxValue = 0;  //Used for normalizing result to 0.0 - 1.0
            if (octaves <= 0) { return 0.0; }
            if (octaves == 1) { return Noise3D(x, y, z); }
            for (int I = 0; I < octaves; I++)
            {
                total += Noise3D(x * frequency, y * frequency, z * frequency) * amplitude;
                maxValue += amplitude;
                amplitude *= persistence;
                frequency *= 2;
            }
            return total / maxValue;
        }

        #endregion

        #region "4 Dimensional Noise"

        /// <summary>
        /// Calculates a single octave widened 4D OpenSimplex Noise value between 0 and 1 for input values x,y,z,w.
        /// </summary>
        /// <param name="x">The x input of the 4D OpenSimplex Noise function.</param>
        /// <param name="y">The y input of the 4D OpenSimplex Noise function.</param>
        /// <param name="z">The z input of the 4D OpenSimplex Noise function.</param>
        /// <param name="w">The w input of the 4D OpenSimplex Noise function.</param>
        /// <param name="factor">The number of times the widening function is applied.</param>
        public static double WideNoise4D(double x, double y, double z, double w, int factor)
        {
            return SmoothStep(Noise4D(x, y, z, w), 0, 1, factor);
        }

        /// <summary>
        /// Calculates a widened 4D OpenSimplex Noise value between 0 and 1 for input values x,y,z,w with given octaves and persistence.
        /// </summary>
        /// <param name="x">The x input of the 4D OpenSimplex Noise function.</param>
        /// <param name="y">The y input of the 4D OpenSimplex Noise function.</param>
        /// <param name="z">The z input of the 4D OpenSimplex Noise function.</param>
        /// <param name="w">The w input of the 4D OpenSimplex Noise function.</param>
        /// <param name="octaves">Number of octaves used to calculate the 4D OpenSimplex Noise.</param>
        /// <param name="persistence">Relative strength of higher octaves in the 4D OpenSimplex Noise function.</param>
        /// <param name="factor">The number of times the widening function is applied.</param>
        public static double WideNoise4D(double x, double y, double z, double w, int octaves, double persistence, int factor)
        {
            return SmoothStep(Noise4D(x, y, z, w, octaves, persistence), 0, 1, factor);
        }

        /// <summary>
        /// Calculates a single octave 4D OpenSimplex Noise value between 0 and 1 for input values x,y,z,w.
        /// </summary>
        /// <param name="x">The x input of the 4D OpenSimplex Noise function.</param>
        /// <param name="y">The y input of the 4D OpenSimplex Noise function.</param>
        /// <param name="z">The z input of the 4D OpenSimplex Noise function.</param>
        /// <param name="w">The w input of the 4D OpenSimplex Noise function.</param>
        public static double Noise4D(double x, double y, double z, double w)
        {
            //Place input coordinates on simplectic honeycomb.
            double stretchOffset = (x + y + z + w) * STRETCH_CONSTANT_4D;
            double xs = x + stretchOffset;
            double ys = y + stretchOffset;
            double zs = z + stretchOffset;
            double ws = w + stretchOffset;
            //Floor to get simplectic honeycomb coordinates of rhombo-hypercube super-cell origin.
            int xsb = FastFloor(xs);
            int ysb = FastFloor(ys);
            int zsb = FastFloor(zs);
            int wsb = FastFloor(ws);
            //Skew out to get actual coordinates of stretched rhombo-hypercube origin. We//ll need these later.
            double squishOffset = (xsb + ysb + zsb + wsb) * SQUISH_CONSTANT_4D;
            double xb = xsb + squishOffset;
            double yb = ysb + squishOffset;
            double zb = zsb + squishOffset;
            double wb = wsb + squishOffset;
            //Compute simplectic honeycomb coordinates relative to rhombo-hypercube origin.
            double xins = xs - xsb;
            double yins = ys - ysb;
            double zins = zs - zsb;
            double wins = ws - wsb;
            //Sum those together to get a value that determines which region we//re in.
            double inSum = xins + yins + zins + wins;
            //Positions relative to origin point.
            double dx0 = x - xb;
            double dy0 = y - yb;
            double dz0 = z - zb;
            double dw0 = w - wb;
            //We//ll be defining these inside the next block and using them afterwards.
            double dx_ext0, dy_ext0, dz_ext0, dw_ext0;
            double dx_ext1, dy_ext1, dz_ext1, dw_ext1;
            double dx_ext2, dy_ext2, dz_ext2, dw_ext2;
            int xsv_ext0, ysv_ext0, zsv_ext0, wsv_ext0;
            int xsv_ext1, ysv_ext1, zsv_ext1, wsv_ext1;
            int xsv_ext2, ysv_ext2, zsv_ext2, wsv_ext2;
            double value = 0;
            if (inSum <= 1) //We're inside the pentachoron(4 - Simplex) at(0, 0, 0, 0)
            {
                //Determine which two of (0,0,0,1), (0,0,1,0), (0,1,0,0), (1,0,0,0) are closest.
                byte aPoint = 0x1;
                double aScore = xins;
                byte bPoint = 0x2;
                double bScore = yins;
                if (aScore >= bScore & zins > bScore) 
                {
                    bScore = zins;
                    bPoint = 0x4;
                } 
                else if (aScore < bScore & zins > aScore) 
                {
                    aScore = zins;
                    aPoint = 0x4;
                }
                if (aScore >= bScore & wins > bScore) 
                {
                    bScore = wins;
                    bPoint = 0x8;
                } 
                else if (aScore < bScore & wins > aScore) 
                {
                    aScore = wins;
                    aPoint = 0x8;
                }
                //Now we determine the three lattice points not part of the pentachoron that may contribute.
                //This depends on the closest two pentachoron vertices, including (0,0,0,0)
                double uins = 1 - inSum;
                if (uins > aScore | uins > bScore) //(0,0,0,0) is one of the closest two pentachoron vertices.
                { 
                    byte c;
                    if (bScore > aScore)  //Our other closest vertex is the closest out of a and b.
                    {
                        c = bPoint;
                    }
                    else 
                    {
                        c = aPoint;
                    }
                    if ((c & 0x1) == 0) 
                    {
                        xsv_ext0 = xsb - 1;
                        xsv_ext1 = xsb;
                        xsv_ext2 = xsb;
                        dx_ext0 = dx0 + 1;
                        dx_ext1 = dx0;
                        dx_ext2 = dx0;
                    }
                    else 
                    {
                        xsv_ext0 = xsb + 1;
                        dx_ext0 = dx0 - 1;
                        xsv_ext1 = xsb + 1;
                        dx_ext1 = dx0 - 1;
                        xsv_ext2 = xsb + 1;
                        dx_ext2 = dx0 - 1;
                    }
                    if ((c & 0x2) == 0) 
                    {
                        ysv_ext0 = ysb;
                        dy_ext0 = dy0;
                        ysv_ext1 = ysb;
                        dy_ext1 = dy0;
                        ysv_ext2 = ysb;
                        dy_ext2 = dy0;
                        if ((c & 0x1) == 0x1) 
                        {
                            ysv_ext0 -= 1;
                            dy_ext0 += 1;
                        } 
                        else {
                            ysv_ext1 -= 1;
                            dy_ext1 += 1;
                        }
                    } 
                    else 
                    {
                        ysv_ext0 = ysb + 1;
                        dy_ext0 = dy0 - 1;
                        ysv_ext1 = ysb + 1;
                        dy_ext1 = dy0 - 1;
                        ysv_ext2 = ysb + 1;
                        dy_ext2 = dy0 - 1;
                    }
                    if ((c & 0x4) == 0) 
                    {
                        zsv_ext0 = zsb;
                        dz_ext0 = dz0;
                        zsv_ext1 = zsb;
                        dz_ext1 = dz0;
                        zsv_ext2 = zsb;
                        dz_ext2 = dz0;
                        if ((c & 0x3) != 0) 
                        {
                            if ((c & 0x3) == 0x3) 
                            {
                                zsv_ext0 -= 1;
                                dz_ext0 += 1;
                            } 
                            else 
                            {
                                zsv_ext1 -= 1;
                                dz_ext1 += 1;
                            }
                        } 
                        else 
                        {
                            zsv_ext2 -= 1;
                            dz_ext2 += 1;
                        }
                    } 
                    else 
                    {
                        zsv_ext0 = zsb + 1;
                        dz_ext0 = dz0 - 1;
                        zsv_ext1 = zsb + 1;
                        dz_ext1 = dz0 - 1;
                        zsv_ext2 = zsb + 1;
                        dz_ext2 = dz0 - 1;
                    }
                    if ((c & 0x8) == 0) 
                    {
                        wsv_ext0 = wsb;
                        wsv_ext1 = wsb;
                        wsv_ext2 = wsb - 1;
                        dw_ext0 = dw0;
                        dw_ext1 = dw0;
                        dw_ext2 = dw0 + 1;
                    } 
                    else 
                    {
                        wsv_ext0 = wsb + 1;
                        dw_ext0 = dw0 - 1;
                        wsv_ext1 = wsb + 1;
                        dw_ext1 = dw0 - 1;
                        wsv_ext2 = wsb + 1;
                        dw_ext2 = dw0 - 1;
                    }
                }
                else //(0,0,0,0) is not one of the closest two pentachoron vertices.
                { 
                    byte c = (byte)(aPoint | bPoint); //Our three extra vertices are determined by the closest two.
                    if ((c & 0x1) == 0) 
                    {
                        xsv_ext0 = xsb;
                        xsv_ext2 = xsb;
                        xsv_ext1 = xsb - 1;
                        dx_ext0 = dx0 - 2 * SQUISH_CONSTANT_4D;
                        dx_ext1 = dx0 + 1 - SQUISH_CONSTANT_4D;
                        dx_ext2 = dx0 - SQUISH_CONSTANT_4D;
                    } 
                    else 
                    {
                        xsv_ext0 = xsb + 1;
                        xsv_ext1 = xsb + 1;
                        xsv_ext2 = xsb + 1;
                        dx_ext0 = dx0 - 1 - 2 * SQUISH_CONSTANT_4D;
                        dx_ext1 = dx0 - 1 - SQUISH_CONSTANT_4D;
                        dx_ext2 = dx0 - 1 - SQUISH_CONSTANT_4D;
                    }
                    if ((c & 0x2) == 0) 
                    {
                        ysv_ext0 = ysb;
                        ysv_ext1 = ysb;
                        ysv_ext2 = ysb;
                        dy_ext0 = dy0 - 2 * SQUISH_CONSTANT_4D;
                        dy_ext1 = dy0 - SQUISH_CONSTANT_4D;
                        dy_ext2 = dy0 - SQUISH_CONSTANT_4D;
                        if ((c & 0x1) == 0x1) 
                        {
                            ysv_ext1 -= 1;
                            dy_ext1 += 1;
                        } 
                        else 
                        {
                            ysv_ext2 -= 1;
                            dy_ext2 += 1;
                        }
                    } 
                    else 
                    {
                        ysv_ext0 = ysb + 1;
                        ysv_ext1 = ysb + 1;
                        ysv_ext2 = ysb + 1;
                        dy_ext0 = dy0 - 1 - 2 * SQUISH_CONSTANT_4D;
                        dy_ext1 = dy0 - 1 - SQUISH_CONSTANT_4D;
                        dy_ext2 = dy0 - 1 - SQUISH_CONSTANT_4D;
                    }
                    if ((c & 0x4) == 0) 
                    {
                        zsv_ext0 = zsb;
                        zsv_ext1 = zsb;
                        zsv_ext2 = zsb;
                        dz_ext0 = dz0 - 2 * SQUISH_CONSTANT_4D;
                        dz_ext1 = dz0 - SQUISH_CONSTANT_4D;
                        dz_ext2 = dz0 - SQUISH_CONSTANT_4D;
                        if ((c & 0x3) == 0x3) 
                        {
                            zsv_ext1 -= 1;
                            dz_ext1 += 1;
                        } 
                        else 
                        {
                            zsv_ext2 -= 1;
                            dz_ext2 += 1;
                        }
                    } 
                    else
                    {
                        zsv_ext0 = zsb + 1;
                        zsv_ext1 = zsb + 1;
                        zsv_ext2 = zsb + 1;
                        dz_ext0 = dz0 - 1 - 2 * SQUISH_CONSTANT_4D;
                        dz_ext1 = dz0 - 1 - SQUISH_CONSTANT_4D;
                        dz_ext2 = dz0 - 1 - SQUISH_CONSTANT_4D;
                    }
                    if ((c & 0x8) == 0) 
                    {
                        wsv_ext0 = wsb;
                        wsv_ext1 = wsb;
                        wsv_ext2 = wsb - 1;
                        dw_ext0 = dw0 - 2 * SQUISH_CONSTANT_4D;
                        dw_ext1 = dw0 - SQUISH_CONSTANT_4D;
                        dw_ext2 = dw0 + 1 - SQUISH_CONSTANT_4D;
                    } 
                    else
                    {
                        wsv_ext0 = wsb + 1;
                        wsv_ext1 = wsb + 1;
                        wsv_ext2 = wsb + 1;
                        dw_ext0 = dw0 - 1 - 2 * SQUISH_CONSTANT_4D;
                        dw_ext1 = dw0 - 1 - SQUISH_CONSTANT_4D;
                        dw_ext2 = dw0 - 1 - SQUISH_CONSTANT_4D;
                    }
                }
                //Contribution (0,0,0,0)
                double attn0 = 2 - dx0 * dx0 - dy0 * dy0 - dz0 * dz0 - dw0 * dw0;
                if (attn0 > 0) 
                {
                    attn0 *= attn0;
                    value += attn0 * attn0 * Extrapolate(xsb + 0, ysb + 0, zsb + 0, wsb + 0, dx0, dy0, dz0, dw0);
                }
                //Contribution (1,0,0,0)
                double dx1 = dx0 - 1 - SQUISH_CONSTANT_4D;
                double dy1 = dy0 - 0 - SQUISH_CONSTANT_4D;
                double dz1 = dz0 - 0 - SQUISH_CONSTANT_4D;
                double dw1 = dw0 - 0 - SQUISH_CONSTANT_4D;
                double attn1 = 2 - dx1 * dx1 - dy1 * dy1 - dz1 * dz1 - dw1 * dw1;
                if (attn1 > 0)
                {
                    attn1 *= attn1;
                    value += attn1 * attn1 * Extrapolate(xsb + 1, ysb + 0, zsb + 0, wsb + 0, dx1, dy1, dz1, dw1);
                }
                //Contribution (0,1,0,0)
                double dx2 = dx0 - 0 - SQUISH_CONSTANT_4D;
                double dy2 = dy0 - 1 - SQUISH_CONSTANT_4D;
                double dz2 = dz1;
                double dw2 = dw1;
                double attn2 = 2 - dx2 * dx2 - dy2 * dy2 - dz2 * dz2 - dw2 * dw2;
                if (attn2 > 0) 
                {
                    attn2 *= attn2;
                    value += attn2 * attn2 * Extrapolate(xsb + 0, ysb + 1, zsb + 0, wsb + 0, dx2, dy2, dz2, dw2);
                }
                //Contribution (0,0,1,0)
                double dx3 = dx2;
                double dy3 = dy1;
                double dz3 = dz0 - 1 - SQUISH_CONSTANT_4D;
                double dw3 = dw1;
                double attn3 = 2 - dx3 * dx3 - dy3 * dy3 - dz3 * dz3 - dw3 * dw3;
                if (attn3 > 0)
                {
                    attn3 *= attn3;
                    value += attn3 * attn3 * Extrapolate(xsb + 0, ysb + 0, zsb + 1, wsb + 0, dx3, dy3, dz3, dw3);
                }
                //Contribution (0,0,0,1)
                double dx4 = dx2;
                double dy4 = dy1;
                double dz4 = dz1;
                double dw4 = dw0 - 1 - SQUISH_CONSTANT_4D;
                double attn4 = 2 - dx4 * dx4 - dy4 * dy4 - dz4 * dz4 - dw4 * dw4;
                if (attn4 > 0) 
                {
                    attn4 *= attn4;
                    value += attn4 * attn4 * Extrapolate(xsb + 0, ysb + 0, zsb + 0, wsb + 1, dx4, dy4, dz4, dw4);
                }
            } 
            else if (inSum >= 3) //We're inside the pentachoron(4 - Simplex) at(1, 1, 1, 1) 
            {
                //Determine which two of (1,1,1,0), (1,1,0,1), (1,0,1,1), (0,1,1,1) are closest.                     
                byte aPoint = 0xE;
                double aScore = xins;
                byte bPoint = 0xD;
                double bScore = yins;
                if (aScore <= bScore & zins < bScore) 
                {
                    bScore = zins;
                    bPoint = 0xB;
                } 
                else if (aScore > bScore & zins < aScore) 
                {
                    aScore = zins;
                    aPoint = 0xB;
                }
                if (aScore <= bScore & wins < bScore) 
                {
                    bScore = wins;
                    bPoint = 0x7;
                }
                else if (aScore > bScore & wins < aScore)
                {
                    aScore = wins;
                    aPoint = 0x7;
                }
                //Now we determine the three lattice points not part of the pentachoron that may contribute.
                //This depends on the closest two pentachoron vertices, including (0,0,0,0)
                double uins = 4 - inSum;
                if (uins < aScore | uins < bScore) //(1,1,1,1) is one of the closest two pentachoron vertices.
                { 
                    byte c;
                    if (bScore < aScore)  //Our other closest vertex is the closest out of a and b.
                    {
                        c = bPoint;
                    } 
                    else 
                    {
                        c = aPoint;
                    }
                    if ((c & 0x1) != 0) 
                    {
                        xsv_ext0 = xsb + 2;
                        xsv_ext1 = xsb + 1;
                        xsv_ext2 = xsb + 1;
                        dx_ext0 = dx0 - 2 - 4 * SQUISH_CONSTANT_4D;
                        dx_ext1 = dx0 - 1 - 4 * SQUISH_CONSTANT_4D;
                        dx_ext2 = dx0 - 1 - 4 * SQUISH_CONSTANT_4D;
                    } 
                    else 
                    {
                        xsv_ext0 = xsb;
                        xsv_ext1 = xsb;
                        xsv_ext2 = xsb;
                        dx_ext0 = dx0 - 4 * SQUISH_CONSTANT_4D;
                        dx_ext1 = dx0 - 4 * SQUISH_CONSTANT_4D;
                        dx_ext2 = dx0 - 4 * SQUISH_CONSTANT_4D;
                    }
                    if ((c & 0x2) != 0) 
                    {
                        ysv_ext0 = ysb + 1;
                        ysv_ext1 = ysb + 1;
                        ysv_ext2 = ysb + 1;
                        dy_ext0 = dy0 - 1 - 4 * SQUISH_CONSTANT_4D;
                        dy_ext1 = dy0 - 1 - 4 * SQUISH_CONSTANT_4D;
                        dy_ext2 = dy0 - 1 - 4 * SQUISH_CONSTANT_4D;
                        if ((c & 0x1) != 0) 
                        {
                            ysv_ext1 += 1;
                            dy_ext1 -= 1;
                        } 
                        else 
                        {
                            ysv_ext0 += 1;
                            dy_ext0 -= 1;
                        }
                    } 
                    else 
                    {
                        ysv_ext0 = ysb;
                        ysv_ext1 = ysb;
                        ysv_ext2 = ysb;
                        dy_ext0 = dy0 - 4 * SQUISH_CONSTANT_4D;
                        dy_ext1 = dy0 - 4 * SQUISH_CONSTANT_4D;
                        dy_ext2 = dy0 - 4 * SQUISH_CONSTANT_4D;
                    }
                    if ((c & 0x4) != 0) 
                    {
                        zsv_ext0 = zsb + 1;
                        zsv_ext1 = zsb + 1;
                        zsv_ext2 = zsb + 1;
                        dz_ext0 = dz0 - 1 - 4 * SQUISH_CONSTANT_4D;
                        dz_ext1 = dz0 - 1 - 4 * SQUISH_CONSTANT_4D;
                        dz_ext2 = dz0 - 1 - 4 * SQUISH_CONSTANT_4D;
                        if ((c & 0x3) != 0x3) 
                        {
                            if ((c & 0x3) == 0)
                            {
                                zsv_ext0 += 1;
                                dz_ext0 -= 1;
                            } 
                            else 
                            {
                                zsv_ext1 += 1;
                                dz_ext1 -= 1;
                            }
                        } 
                        else 
                        {
                            zsv_ext2 += 1;
                            dz_ext2 -= 1;
                        }
                    } 
                    else 
                    {
                        zsv_ext0 = zsb;
                        zsv_ext1 = zsb;
                        zsv_ext2 = zsb;
                        dz_ext0 = dz0 - 4 * SQUISH_CONSTANT_4D;
                        dz_ext1 = dz0 - 4 * SQUISH_CONSTANT_4D;
                        dz_ext2 = dz0 - 4 * SQUISH_CONSTANT_4D;
                    }
                    if ((c & 0x8) != 0) 
                    {
                        wsv_ext0 = wsb + 1;
                        wsv_ext1 = wsb + 1;
                        wsv_ext2 = wsb + 2;
                        dw_ext0 = dw0 - 1 - 4 * SQUISH_CONSTANT_4D;
                        dw_ext1 = dw0 - 1 - 4 * SQUISH_CONSTANT_4D;
                        dw_ext2 = dw0 - 2 - 4 * SQUISH_CONSTANT_4D;
                    }
                    else
                    {
                        wsv_ext0 = wsb;
                        wsv_ext1 = wsb;
                        wsv_ext2 = wsb;
                        dw_ext0 = dw0 - 4 * SQUISH_CONSTANT_4D;
                        dw_ext1 = dw0 - 4 * SQUISH_CONSTANT_4D;
                        dw_ext2 = dw0 - 4 * SQUISH_CONSTANT_4D;
                    }
                }
                else //(1,1,1,1) is not one of the closest two pentachoron vertices. 
                { 
                    byte c = (byte)(aPoint & bPoint); //Our three extra vertices are determined by the closest two.
                    if ((c & 0x1) != 0) 
                    {
                        xsv_ext0 = xsb + 1;
                        xsv_ext2 = xsb + 1;
                        xsv_ext1 = xsb + 2;
                        dx_ext0 = dx0 - 1 - 2 * SQUISH_CONSTANT_4D;
                        dx_ext1 = dx0 - 2 - 3 * SQUISH_CONSTANT_4D;
                        dx_ext2 = dx0 - 1 - 3 * SQUISH_CONSTANT_4D;
                    } 
                    else 
                    {
                        xsv_ext0 = xsb;
                        xsv_ext1 = xsb;
                        xsv_ext2 = xsb;
                        dx_ext0 = dx0 - 2 * SQUISH_CONSTANT_4D;
                        dx_ext1 = dx0 - 3 * SQUISH_CONSTANT_4D;
                        dx_ext2 = dx0 - 3 * SQUISH_CONSTANT_4D;
                    }
                    if ((c & 0x2) != 0) 
                    {
                        ysv_ext0 = ysb + 1;
                        ysv_ext1 = ysb + 1;
                        ysv_ext2 = ysb + 1;
                        dy_ext0 = dy0 - 1 - 2 * SQUISH_CONSTANT_4D;
                        dy_ext1 = dy0 - 1 - 3 * SQUISH_CONSTANT_4D;
                        dy_ext2 = dy0 - 1 - 3 * SQUISH_CONSTANT_4D;
                        if ((c & 0x1) != 0) 
                        {
                            ysv_ext2 += 1;
                            dy_ext2 -= 1;
                        } 
                        else 
                        {
                            ysv_ext1 += 1;
                            dy_ext1 -= 1;
                        }
                    } 
                    else 
                    {
                        ysv_ext0 = ysb;
                        ysv_ext1 = ysb;
                        ysv_ext2 = ysb;
                        dy_ext0 = dy0 - 2 * SQUISH_CONSTANT_4D;
                        dy_ext1 = dy0 - 3 * SQUISH_CONSTANT_4D;
                        dy_ext2 = dy0 - 3 * SQUISH_CONSTANT_4D;
                    }
                    if ((c & 0x4) != 0) 
                    {
                        zsv_ext0 = zsb + 1;
                        zsv_ext1 = zsb + 1;
                        zsv_ext2 = zsb + 1;
                        dz_ext0 = dz0 - 1 - 2 * SQUISH_CONSTANT_4D;
                        dz_ext1 = dz0 - 1 - 3 * SQUISH_CONSTANT_4D;
                        dz_ext2 = dz0 - 1 - 3 * SQUISH_CONSTANT_4D;
                        if ((c & 0x3) != 0) 
                        {
                            zsv_ext2 += 1;
                            dz_ext2 -= 1;
                        }
                        else 
                        {
                            zsv_ext1 += 1;
                            dz_ext1 -= 1;
                        }
                    } 
                    else 
                    {
                        zsv_ext0 = zsb;
                        zsv_ext1 = zsb;
                        zsv_ext2 = zsb;
                        dz_ext0 = dz0 - 2 * SQUISH_CONSTANT_4D;
                        dz_ext1 = dz0 - 3 * SQUISH_CONSTANT_4D;
                        dz_ext2 = dz0 - 3 * SQUISH_CONSTANT_4D;
                    }
                    if ((c & 0x8) != 0) 
                    {
                        wsv_ext0 = wsb + 1;
                        wsv_ext1 = wsb + 1;
                        wsv_ext2 = wsb + 2;
                        dw_ext0 = dw0 - 1 - 2 * SQUISH_CONSTANT_4D;
                        dw_ext1 = dw0 - 1 - 3 * SQUISH_CONSTANT_4D;
                        dw_ext2 = dw0 - 2 - 3 * SQUISH_CONSTANT_4D;
                    } 
                    else 
                    {
                        wsv_ext0 = wsb;
                        wsv_ext1 = wsb;
                        wsv_ext2 = wsb;
                        dw_ext0 = dw0 - 2 * SQUISH_CONSTANT_4D;
                        dw_ext1 = dw0 - 3 * SQUISH_CONSTANT_4D;
                        dw_ext2 = dw0 - 3 * SQUISH_CONSTANT_4D;
                    }
                }
                //Contribution (1,1,1,0)
                double dx4 = dx0 - 1 - 3 * SQUISH_CONSTANT_4D;
                double dy4 = dy0 - 1 - 3 * SQUISH_CONSTANT_4D;
                double dz4 = dz0 - 1 - 3 * SQUISH_CONSTANT_4D;
                double dw4 = dw0 - 3 * SQUISH_CONSTANT_4D;
                double attn4 = 2 - dx4 * dx4 - dy4 * dy4 - dz4 * dz4 - dw4 * dw4;
                if (attn4 > 0) 
                {
                    attn4 *= attn4;
                    value += attn4 * attn4 * Extrapolate(xsb + 1, ysb + 1, zsb + 1, wsb + 0, dx4, dy4, dz4, dw4);
                }
                //Contribution (1,1,0,1)
                double dx3 = dx4;
                double dy3 = dy4;
                double dz3 = dz0 - 3 * SQUISH_CONSTANT_4D;
                double dw3 = dw0 - 1 - 3 * SQUISH_CONSTANT_4D;
                double attn3 = 2 - dx3 * dx3 - dy3 * dy3 - dz3 * dz3 - dw3 * dw3;
                if (attn3 > 0) 
                {
                    attn3 *= attn3;
                    value += attn3 * attn3 * Extrapolate(xsb + 1, ysb + 1, zsb + 0, wsb + 1, dx3, dy3, dz3, dw3);
                }
                //Contribution (1,0,1,1)
                double dx2 = dx4;
                double dy2 = dy0 - 3 * SQUISH_CONSTANT_4D;
                double dz2 = dz4;
                double dw2 = dw3;
                double attn2 = 2 - dx2 * dx2 - dy2 * dy2 - dz2 * dz2 - dw2 * dw2;
                if (attn2 > 0) 
                {
                    attn2 *= attn2;
                    value += attn2 * attn2 * Extrapolate(xsb + 1, ysb + 0, zsb + 1, wsb + 1, dx2, dy2, dz2, dw2);
                }
                //Contribution (0,1,1,1)
                double dx1 = dx0 - 3 * SQUISH_CONSTANT_4D;
                double dz1 = dz4;
                double dy1 = dy4;
                double dw1 = dw3;
                double attn1 = 2 - dx1 * dx1 - dy1 * dy1 - dz1 * dz1 - dw1 * dw1;
                if (attn1 > 0) 
                {
                    attn1 *= attn1;
                    value += attn1 * attn1 * Extrapolate(xsb + 0, ysb + 1, zsb + 1, wsb + 1, dx1, dy1, dz1, dw1);
                }
                //Contribution (1,1,1,1)
                dx0 = dx0 - 1 - 4 * SQUISH_CONSTANT_4D;
                dy0 = dy0 - 1 - 4 * SQUISH_CONSTANT_4D;
                dz0 = dz0 - 1 - 4 * SQUISH_CONSTANT_4D;
                dw0 = dw0 - 1 - 4 * SQUISH_CONSTANT_4D;
                double attn0 = 2 - dx0 * dx0 - dy0 * dy0 - dz0 * dz0 - dw0 * dw0;
                if (attn0 > 0)
                {
                    attn0 *= attn0;
                    value += attn0 * attn0 * Extrapolate(xsb + 1, ysb + 1, zsb + 1, wsb + 1, dx0, dy0, dz0, dw0);
                }
            } 
            else if (inSum <= 2) //We're inside the first dispentachoron(Rectified 4 - Simplex)
            { 
                double aScore;
                byte aPoint;
                bool aIsBiggerSide = true;
                double bScore;
                byte bPoint;
                bool bIsBiggerSide = true;
                //Decide between (1,1,0,0) and (0,0,1,1)
                if (xins + yins > zins + wins) 
                {
                    aScore = xins + yins;
                    aPoint = 0x3;
                } 
                else 
                {
                    aScore = zins + wins;
                    aPoint = 0xC;
                }
                //Decide between (1,0,1,0) and (0,1,0,1)
                if (xins + zins > yins + wins) 
                {
                    bScore = xins + zins;
                    bPoint = 0x5;
                }
                else 
                {
                    bScore = yins + wins;
                    bPoint = 0xA;
                }
                //Closer between (1,0,0,1) and (0,1,1,0) will replace the further of a and b, if closer.
                if (xins + wins > yins + zins) 
                {
                    double score = xins + wins;
                    if (aScore >= bScore & score > bScore) 
                    {
                        bScore = score;
                        bPoint = 0x9;
                    } 
                    else if (aScore < bScore & score > aScore) 
                    {
                        aScore = score;
                        aPoint = 0x9;
                    }
                } 
                else 
                {
                    double score = yins + zins;
                    if (aScore >= bScore & score > bScore) 
                    {
                        bScore = score;
                        bPoint = 0x6;
                    }
                    else if (aScore < bScore & score > aScore) 
                    {
                        aScore = score;
                        aPoint = 0x6;
                    }
                }
                //Decide if (1,0,0,0) is closer.
                double p1 = 2 - inSum + xins;
                if (aScore >= bScore & p1 > bScore) 
                {
                    bScore = p1;
                    bPoint = 0x1;
                    bIsBiggerSide = false;
                }
                else if (aScore < bScore & p1 > aScore) 
                {
                    aScore = p1;
                    aPoint = 0x1;
                    aIsBiggerSide = false;
                }
                //Decide if (0,1,0,0) is closer.
                double p2 = 2 - inSum + yins;
                if (aScore >= bScore & p2 > bScore) 
                {
                    bScore = p2;
                    bPoint = 0x2;
                    bIsBiggerSide = false;
                }
                else if (aScore < bScore & p2 > aScore) 
                {
                    aScore = p2;
                    aPoint = 0x2;
                    aIsBiggerSide = false;
                }
                //Decide if (0,0,1,0) is closer.
                double p3 = 2 - inSum + zins;
                if (aScore >= bScore & p3 > bScore) 
                {
                    bScore = p3;
                    bPoint = 0x4;
                    bIsBiggerSide = false;
                }
                else if (aScore < bScore & p3 > aScore) 
                {
                    aScore = p3;
                    aPoint = 0x4;
                    aIsBiggerSide = false;
                }
                //Decide if (0,0,0,1) is closer.
                double p4 = 2 - inSum + wins;
                if (aScore >= bScore & p4 > bScore) 
                {
                    bScore = p4;
                    bPoint = 0x8;
                    bIsBiggerSide = false;
                }
                else if (aScore < bScore & p4 > aScore) 
                {
                    aScore = p4;
                    aPoint = 0x8;
                    aIsBiggerSide = false;
                }
                //Where each of the two closest points are, determines how the extra three vertices are calculated.
                if (aIsBiggerSide == bIsBiggerSide) 
                {
                    if (aIsBiggerSide) 
                    {
                        //Both closest points on the bigger side
                        byte c1 = (byte)(aPoint | bPoint);
                        byte c2 = (byte)(aPoint & bPoint);
                        if ((c1 & 0x1) == 0) 
                        {
                            xsv_ext0 = xsb;
                            xsv_ext1 = xsb - 1;
                            dx_ext0 = dx0 - 3 * SQUISH_CONSTANT_4D;
                            dx_ext1 = dx0 + 1 - 2 * SQUISH_CONSTANT_4D;
                        } 
                        else 
                        {
                            xsv_ext0 = xsb + 1;
                            xsv_ext1 = xsb + 1;
                            dx_ext0 = dx0 - 1 - 3 * SQUISH_CONSTANT_4D;
                            dx_ext1 = dx0 - 1 - 2 * SQUISH_CONSTANT_4D;
                        }
                        if ((c1 & 0x2) == 0) 
                        {
                            ysv_ext0 = ysb;
                            ysv_ext1 = ysb - 1;
                            dy_ext0 = dy0 - 3 * SQUISH_CONSTANT_4D;
                            dy_ext1 = dy0 + 1 - 2 * SQUISH_CONSTANT_4D;
                        } 
                        else 
                        {
                            ysv_ext0 = ysb + 1;
                            ysv_ext1 = ysb + 1;
                            dy_ext0 = dy0 - 1 - 3 * SQUISH_CONSTANT_4D;
                            dy_ext1 = dy0 - 1 - 2 * SQUISH_CONSTANT_4D;
                        }
                        if ((c1 & 0x4) == 0) 
                        {
                            zsv_ext0 = zsb;
                            zsv_ext1 = zsb - 1;
                            dz_ext0 = dz0 - 3 * SQUISH_CONSTANT_4D;
                            dz_ext1 = dz0 + 1 - 2 * SQUISH_CONSTANT_4D;
                        }
                        else 
                        {
                            zsv_ext0 = zsb + 1;
                            zsv_ext1 = zsb + 1;
                            dz_ext0 = dz0 - 1 - 3 * SQUISH_CONSTANT_4D;
                            dz_ext1 = dz0 - 1 - 2 * SQUISH_CONSTANT_4D;
                        }
                        if ((c1 & 0x8) == 0) 
                        {
                            wsv_ext0 = wsb;
                            wsv_ext1 = wsb - 1;
                            dw_ext0 = dw0 - 3 * SQUISH_CONSTANT_4D;
                            dw_ext1 = dw0 + 1 - 2 * SQUISH_CONSTANT_4D;
                        }
                        else 
                        {
                            wsv_ext0 = wsb + 1;
                            wsv_ext1 = wsb + 1;
                            dw_ext0 = dw0 - 1 - 3 * SQUISH_CONSTANT_4D;
                            dw_ext1 = dw0 - 1 - 2 * SQUISH_CONSTANT_4D;
                        }
                        //One combination is a permutation of (0,0,0,2) based on c2
                        xsv_ext2 = xsb;
                        ysv_ext2 = ysb;
                        zsv_ext2 = zsb;
                        wsv_ext2 = wsb;
                        dx_ext2 = dx0 - 2 * SQUISH_CONSTANT_4D;
                        dy_ext2 = dy0 - 2 * SQUISH_CONSTANT_4D;
                        dz_ext2 = dz0 - 2 * SQUISH_CONSTANT_4D;
                        dw_ext2 = dw0 - 2 * SQUISH_CONSTANT_4D;
                        if ((c2 & 0x1) != 0) 
                        {
                            xsv_ext2 += 2;
                            dx_ext2 -= 2;
                        }
                        else if ((c2 & 0x2) != 0) 
                        {
                            ysv_ext2 += 2;
                            dy_ext2 -= 2;
                        }
                        else if ((c2 & 0x4) != 0) 
                        {
                            zsv_ext2 += 2;
                            dz_ext2 -= 2;
                        }
                        else 
                        {
                            wsv_ext2 += 2;
                            dw_ext2 -= 2;
                        }
                    } 
                    else 
                    {
                        //Both closest points on the smaller side
                        //One of the two extra points is (0,0,0,0)
                        xsv_ext2 = xsb;
                        ysv_ext2 = ysb;
                        zsv_ext2 = zsb;
                        wsv_ext2 = wsb;
                        dx_ext2 = dx0;
                        dy_ext2 = dy0;
                        dz_ext2 = dz0;
                        dw_ext2 = dw0;
                        //Other two points are based on the omitted axes.
                        byte c = (byte)(aPoint | bPoint);
                        if ((c & 0x1) == 0) 
                        {
                            xsv_ext0 = xsb - 1;
                            xsv_ext1 = xsb;
                            dx_ext0 = dx0 + 1 - SQUISH_CONSTANT_4D;
                            dx_ext1 = dx0 - SQUISH_CONSTANT_4D;
                        } 
                        else 
                        {
                            xsv_ext0 = xsb + 1;
                            xsv_ext1 = xsb + 1;
                            dx_ext0 = dx0 - 1 - SQUISH_CONSTANT_4D;
                            dx_ext1 = dx0 - 1 - SQUISH_CONSTANT_4D;
                        }
                        if ((c & 0x2) == 0) 
                        {
                            ysv_ext0 = ysb;
                            ysv_ext1 = ysb;
                            dy_ext0 = dy0 - SQUISH_CONSTANT_4D;
                            dy_ext1 = dy0 - SQUISH_CONSTANT_4D;
                            if ((c & 0x1) == 0x1) 
                            {
                                ysv_ext0 -= 1;
                                dy_ext0 += 1;
                            }
                            else
                            {
                                ysv_ext1 -= 1;
                                dy_ext1 += 1;
                            }
                        } 
                        else 
                        {
                            ysv_ext0 = ysb + 1;
                            ysv_ext1 = ysb + 1;
                            dy_ext0 = dy0 - 1 - SQUISH_CONSTANT_4D;
                            dy_ext1 = dy0 - 1 - SQUISH_CONSTANT_4D;
                        }
                        if ((c & 0x4) == 0) 
                        {
                            zsv_ext0 = zsb;
                            zsv_ext1 = zsb;
                            dz_ext0 = dz0 - SQUISH_CONSTANT_4D;
                            dz_ext1 = dz0 - SQUISH_CONSTANT_4D;
                            if ((c & 0x3) == 0x3) 
                            {
                                zsv_ext0 -= 1;
                                dz_ext0 += 1;
                            } 
                            else 
                            {
                                zsv_ext1 -= 1;
                                dz_ext1 += 1;
                            }
                        } 
                        else 
                        {
                            zsv_ext0 = zsb + 1;
                            zsv_ext1 = zsb + 1;
                            dz_ext0 = dz0 - 1 - SQUISH_CONSTANT_4D;
                            dz_ext1 = dz0 - 1 - SQUISH_CONSTANT_4D;
                        }
                        if ((c & 0x8) == 0) 
                        {
                            wsv_ext0 = wsb;
                            wsv_ext1 = wsb - 1;
                            dw_ext0 = dw0 - SQUISH_CONSTANT_4D;
                            dw_ext1 = dw0 + 1 - SQUISH_CONSTANT_4D;
                        }
                        else 
                        {
                            wsv_ext0 = wsb + 1;
                            wsv_ext1 = wsb + 1;
                            dw_ext0 = dw0 - 1 - SQUISH_CONSTANT_4D;
                            dw_ext1 = dw0 - 1 - SQUISH_CONSTANT_4D;
                        }
                    }
                } 
                else //One point on each "side"
                {  
                    byte c1, c2;
                    if (aIsBiggerSide) 
                    {
                        c1 = aPoint;
                        c2 = bPoint;
                    }
                    else 
                    {
                        c1 = bPoint;
                        c2 = aPoint;
                    }
                    //Two contributions are the bigger-sided point with each 0 replaced with -1.
                    if ((c1 & 0x1) == 0) 
                    {
                        xsv_ext0 = xsb - 1;
                        xsv_ext1 = xsb;
                        dx_ext0 = dx0 + 1 - SQUISH_CONSTANT_4D;
                        dx_ext1 = dx0 - SQUISH_CONSTANT_4D;
                    }
                    else 
                    {
                        xsv_ext0 = xsb + 1;
                        xsv_ext1 = xsb + 1;
                        dx_ext0 = dx0 - 1 - SQUISH_CONSTANT_4D;
                        dx_ext1 = dx0 - 1 - SQUISH_CONSTANT_4D;
                    }
                    if ((c1 & 0x2) == 0) 
                    {
                        ysv_ext0 = ysb;
                        ysv_ext1 = ysb;
                        dy_ext0 = dy0 - SQUISH_CONSTANT_4D;
                        dy_ext1 = dy0 - SQUISH_CONSTANT_4D;
                        if ((c1 & 0x1) == 0x1) 
                        {
                            ysv_ext0 -= 1;
                            dy_ext0 += 1;
                        }
                        else 
                        {
                            ysv_ext1 -= 1;
                            dy_ext1 += 1;
                        }
                    } 
                    else 
                    {
                        ysv_ext0 = ysb + 1;
                        ysv_ext1 = ysb + 1;
                        dy_ext0 = dy0 - 1 - SQUISH_CONSTANT_4D;
                        dy_ext1 = dy0 - 1 - SQUISH_CONSTANT_4D;
                    }
                    if ((c1 & 0x4) == 0) 
                    {
                        zsv_ext0 = zsb;
                        zsv_ext1 = zsb;
                        dz_ext0 = dz0 - SQUISH_CONSTANT_4D;
                        dz_ext1 = dz0 - SQUISH_CONSTANT_4D;
                        if ((c1 & 0x3) == 0x3) 
                        {
                            zsv_ext0 -= 1;
                            dz_ext0 += 1;
                        }
                        else
                        {
                            zsv_ext1 -= 1;
                            dz_ext1 += 1;
                        }
                    } 
                    else 
                    {
                        zsv_ext0 = zsb + 1;
                        zsv_ext1 = zsb + 1;
                        dz_ext0 = dz0 - 1 - SQUISH_CONSTANT_4D;
                        dz_ext1 = dz0 - 1 - SQUISH_CONSTANT_4D;
                    }
                    if ((c1 & 0x8) == 0) 
                    {
                        wsv_ext0 = wsb;
                        wsv_ext1 = wsb - 1;
                        dw_ext0 = dw0 - SQUISH_CONSTANT_4D;
                        dw_ext1 = dw0 + 1 - SQUISH_CONSTANT_4D;
                    }
                    else 
                    {
                        wsv_ext0 = wsb + 1;
                        wsv_ext1 = wsb + 1;
                        dw_ext0 = dw0 - 1 - SQUISH_CONSTANT_4D;
                        dw_ext1 = dw0 - 1 - SQUISH_CONSTANT_4D;
                    }
                    //One contribution is a permutation of (0,0,0,2) based on the smaller-sided point
                    xsv_ext2 = xsb;
                    ysv_ext2 = ysb;
                    zsv_ext2 = zsb;
                    wsv_ext2 = wsb;
                    dx_ext2 = dx0 - 2 * SQUISH_CONSTANT_4D;
                    dy_ext2 = dy0 - 2 * SQUISH_CONSTANT_4D;
                    dz_ext2 = dz0 - 2 * SQUISH_CONSTANT_4D;
                    dw_ext2 = dw0 - 2 * SQUISH_CONSTANT_4D;
                    if ((c2 & 0x1) != 0) 
                    {
                        xsv_ext2 += 2;
                        dx_ext2 -= 2;
                    }
                    else if ((c2 & 0x2) != 0) 
                    {
                        ysv_ext2 += 2;
                        dy_ext2 -= 2;
                    }
                    else if ((c2 & 0x4) != 0) 
                    {
                        zsv_ext2 += 2;
                        dz_ext2 -= 2;

                    }
                    else
                    {
                        wsv_ext2 += 2;
                        dw_ext2 -= 2;
                    }
                }
                //Contribution (1,0,0,0)
                double dx1 = dx0 - 1 - SQUISH_CONSTANT_4D;
                double dy1 = dy0 - 0 - SQUISH_CONSTANT_4D;
                double dz1 = dz0 - 0 - SQUISH_CONSTANT_4D;
                double dw1 = dw0 - 0 - SQUISH_CONSTANT_4D;
                double attn1 = 2 - dx1 * dx1 - dy1 * dy1 - dz1 * dz1 - dw1 * dw1;
                if (attn1 > 0) 
                {
                    attn1 *= attn1;
                    value += attn1 * attn1 * Extrapolate(xsb + 1, ysb + 0, zsb + 0, wsb + 0, dx1, dy1, dz1, dw1);
                }
                //Contribution (0,1,0,0)
                double dx2 = dx0 - 0 - SQUISH_CONSTANT_4D;
                double dy2 = dy0 - 1 - SQUISH_CONSTANT_4D;
                double dz2 = dz1;
                double dw2 = dw1;
                double attn2 = 2 - dx2 * dx2 - dy2 * dy2 - dz2 * dz2 - dw2 * dw2;
                if (attn2 > 0) 
                {
                    attn2 *= attn2;
                    value += attn2 * attn2 * Extrapolate(xsb + 0, ysb + 1, zsb + 0, wsb + 0, dx2, dy2, dz2, dw2);
                }
                //Contribution (0,0,1,0)
                double dx3 = dx2;
                double dy3 = dy1;
                double dz3 = dz0 - 1 - SQUISH_CONSTANT_4D;
                double dw3 = dw1;
                double attn3 = 2 - dx3 * dx3 - dy3 * dy3 - dz3 * dz3 - dw3 * dw3;
                if (attn3 > 0) 
                {
                    attn3 *= attn3;
                    value += attn3 * attn3 * Extrapolate(xsb + 0, ysb + 0, zsb + 1, wsb + 0, dx3, dy3, dz3, dw3);
                }
                //Contribution (0,0,0,1)
                double dx4 = dx2;
                double dy4 = dy1;
                double dz4 = dz1;
                double dw4 = dw0 - 1 - SQUISH_CONSTANT_4D;
                double attn4 = 2 - dx4 * dx4 - dy4 * dy4 - dz4 * dz4 - dw4 * dw4;
                if (attn4 > 0)
                {
                    attn4 *= attn4;
                    value += attn4 * attn4 * Extrapolate(xsb + 0, ysb + 0, zsb + 0, wsb + 1, dx4, dy4, dz4, dw4);
                }
                //Contribution (1,1,0,0)
                double dx5 = dx0 - 1 - 2 * SQUISH_CONSTANT_4D;
                double dy5 = dy0 - 1 - 2 * SQUISH_CONSTANT_4D;
                double dz5 = dz0 - 0 - 2 * SQUISH_CONSTANT_4D;
                double dw5 = dw0 - 0 - 2 * SQUISH_CONSTANT_4D;
                double attn5 = 2 - dx5 * dx5 - dy5 * dy5 - dz5 * dz5 - dw5 * dw5;
                if (attn5 > 0) 
                {
                    attn5 *= attn5;
                    value += attn5 * attn5 * Extrapolate(xsb + 1, ysb + 1, zsb + 0, wsb + 0, dx5, dy5, dz5, dw5);
                }
                //Contribution (1,0,1,0)
                double dx6 = dx0 - 1 - 2 * SQUISH_CONSTANT_4D;
                double dy6 = dy0 - 0 - 2 * SQUISH_CONSTANT_4D;
                double dz6 = dz0 - 1 - 2 * SQUISH_CONSTANT_4D;
                double dw6 = dw0 - 0 - 2 * SQUISH_CONSTANT_4D;
                double attn6 = 2 - dx6 * dx6 - dy6 * dy6 - dz6 * dz6 - dw6 * dw6;
                if (attn6 > 0) 
                {
                    attn6 *= attn6;
                    value += attn6 * attn6 * Extrapolate(xsb + 1, ysb + 0, zsb + 1, wsb + 0, dx6, dy6, dz6, dw6);
                }
                //Contribution (1,0,0,1)
                double dx7 = dx0 - 1 - 2 * SQUISH_CONSTANT_4D;
                double dy7 = dy0 - 0 - 2 * SQUISH_CONSTANT_4D;
                double dz7 = dz0 - 0 - 2 * SQUISH_CONSTANT_4D;
                double dw7 = dw0 - 1 - 2 * SQUISH_CONSTANT_4D;
                double attn7 = 2 - dx7 * dx7 - dy7 * dy7 - dz7 * dz7 - dw7 * dw7;
                if (attn7 > 0) 
                {
                    attn7 *= attn7;
                    value += attn7 * attn7 * Extrapolate(xsb + 1, ysb + 0, zsb + 0, wsb + 1, dx7, dy7, dz7, dw7);
                }
                //Contribution (0,1,1,0)
                double dx8 = dx0 - 0 - 2 * SQUISH_CONSTANT_4D;
                double dy8 = dy0 - 1 - 2 * SQUISH_CONSTANT_4D;
                double dz8 = dz0 - 1 - 2 * SQUISH_CONSTANT_4D;
                double dw8 = dw0 - 0 - 2 * SQUISH_CONSTANT_4D;
                double attn8 = 2 - dx8 * dx8 - dy8 * dy8 - dz8 * dz8 - dw8 * dw8;
                if (attn8 > 0) 
                {
                    attn8 *= attn8;
                    value += attn8 * attn8 * Extrapolate(xsb + 0, ysb + 1, zsb + 1, wsb + 0, dx8, dy8, dz8, dw8);
                }
                //Contribution (0,1,0,1)
                double dx9 = dx0 - 0 - 2 * SQUISH_CONSTANT_4D;
                double dy9 = dy0 - 1 - 2 * SQUISH_CONSTANT_4D;
                double dz9 = dz0 - 0 - 2 * SQUISH_CONSTANT_4D;
                double dw9 = dw0 - 1 - 2 * SQUISH_CONSTANT_4D;
                double attn9 = 2 - dx9 * dx9 - dy9 * dy9 - dz9 * dz9 - dw9 * dw9;
                if (attn9 > 0) 
                {
                    attn9 *= attn9;
                    value += attn9 * attn9 * Extrapolate(xsb + 0, ysb + 1, zsb + 0, wsb + 1, dx9, dy9, dz9, dw9);
                }
                //Contribution (0,0,1,1)
                double dx10 = dx0 - 0 - 2 * SQUISH_CONSTANT_4D;
                double dy10 = dy0 - 0 - 2 * SQUISH_CONSTANT_4D;
                double dz10 = dz0 - 1 - 2 * SQUISH_CONSTANT_4D;
                double dw10 = dw0 - 1 - 2 * SQUISH_CONSTANT_4D;
                double attn10 = 2 - dx10 * dx10 - dy10 * dy10 - dz10 * dz10 - dw10 * dw10;
                if (attn10 > 0) 
                {
                    attn10 *= attn10;
                    value += attn10 * attn10 * Extrapolate(xsb + 0, ysb + 0, zsb + 1, wsb + 1, dx10, dy10, dz10, dw10);
                }
            }
            else  //We're inside the second dispentachoron(Rectified 4 - Simplex)
            {
                double aScore;
                byte aPoint;
                bool aIsBiggerSide = true;
                double bScore;
                byte bPoint;
                bool bIsBiggerSide = true;
                //Decide between (0,0,1,1) and (1,1,0,0)
                if (xins + yins < zins + wins)
                {
                    aScore = xins + yins;
                    aPoint = 0xC;
                }
                else 
                {
                    aScore = zins + wins;
                    aPoint = 0x3;
                }
                //Decide between (0,1,0,1) and (1,0,1,0)
                if (xins + zins < yins + wins) 
                {
                    bScore = xins + zins;
                    bPoint = 0xA;
                }
                else 
                {
                    bScore = yins + wins;
                    bPoint = 0x5;
                }
                //Closer between (0,1,1,0) and (1,0,0,1) will replace the further of a and b, if closer.
                if (xins + wins < yins + zins) 
                {
                    double score = xins + wins;
                    if (aScore <= bScore & score < bScore) 
                    {
                        bScore = score;
                        bPoint = 0x6;
                    }
                    else if (aScore > bScore & score < aScore) 
                    {
                        aScore = score;
                        aPoint = 0x6;
                    }
                } 
                else 
                {
                    double score = yins + zins;
                    if (aScore <= bScore & score < bScore) 
                    {
                        bScore = score;
                        bPoint = 0x9;
                    }
                    else if (aScore > bScore & score < aScore) 
                    {
                        aScore = score;
                        aPoint = 0x9;
                    }
                }
                //Decide if (0,1,1,1) is closer.
                double p1 = 3 - inSum + xins;
                if (aScore <= bScore & p1 < bScore) 
                {
                    bScore = p1;
                    bPoint = 0xE;
                    bIsBiggerSide = false;
                }
                else if (aScore > bScore & p1 < aScore) 
                {
                    aScore = p1;
                    aPoint = 0xE;
                    aIsBiggerSide = false;
                }
                //Decide if (1,0,1,1) is closer.
                double p2 = 3 - inSum + yins;
                if (aScore <= bScore & p2 < bScore) 
                {
                    bScore = p2;
                    bPoint = 0xD;
                    bIsBiggerSide = false;
                }
                else if (aScore > bScore & p2 < aScore) 
                {
                    aScore = p2;
                    aPoint = 0xD;
                    aIsBiggerSide = false;
                }
                //Decide if (1,1,0,1) is closer.
                double p3 = 3 - inSum + zins;
                if (aScore <= bScore & p3 < bScore) 
                {
                    bScore = p3;
                    bPoint = 0xB;
                    bIsBiggerSide = false;
                }
                else if (aScore > bScore & p3 < aScore) 
                {
                    aScore = p3;
                    aPoint = 0xB;
                    aIsBiggerSide = false;
                }
                //Decide if (1,1,1,0) is closer.
                double p4 = 3 - inSum + wins;
                if (aScore <= bScore & p4 < bScore) 
                {
                    bScore = p4;
                    bPoint = 0x7;
                    bIsBiggerSide = false;
                }
                else if (aScore > bScore & p4 < aScore) 
                {
                    aScore = p4;
                    aPoint = 0x7;
                    aIsBiggerSide = false;
                }
                //Where each of the two closest points are determines how the extra three vertices are calculated.
                if (aIsBiggerSide == bIsBiggerSide) 
                {
                    if (aIsBiggerSide) 
                    {
                        //Both closest points on the bigger side
                        byte c1 = (byte)(aPoint & bPoint);
                        byte c2 = (byte)(aPoint | bPoint);
                        //Two contributions are permutations of (0,0,0,1) and (0,0,0,2) based on c1
                        xsv_ext0 = xsb;
                        xsv_ext1 = xsb;
                        ysv_ext0 = ysb;
                        ysv_ext1 = ysb;
                        zsv_ext0 = zsb;
                        zsv_ext1 = zsb;
                        wsv_ext0 = wsb;
                        wsv_ext1 = wsb;
                        dx_ext0 = dx0 - SQUISH_CONSTANT_4D;
                        dy_ext0 = dy0 - SQUISH_CONSTANT_4D;
                        dz_ext0 = dz0 - SQUISH_CONSTANT_4D;
                        dw_ext0 = dw0 - SQUISH_CONSTANT_4D;
                        dx_ext1 = dx0 - 2 * SQUISH_CONSTANT_4D;
                        dy_ext1 = dy0 - 2 * SQUISH_CONSTANT_4D;
                        dz_ext1 = dz0 - 2 * SQUISH_CONSTANT_4D;
                        dw_ext1 = dw0 - 2 * SQUISH_CONSTANT_4D;
                        if ((c1 & 0x1) != 0) 
                        {
                            xsv_ext0 += 1;
                            dx_ext0 -= 1;
                            xsv_ext1 += 2;
                            dx_ext1 -= 2;
                        }
                        else if ((c1 & 0x2) != 0) 
                        {
                            ysv_ext0 += 1;
                            dy_ext0 -= 1;
                            ysv_ext1 += 2;
                            dy_ext1 -= 2;
                        }
                        else if ((c1 & 0x4) != 0) 
                        {
                            zsv_ext0 += 1;
                            dz_ext0 -= 1;
                            zsv_ext1 += 2;
                            dz_ext1 -= 2;
                        }
                        else 
                        {
                            wsv_ext0 += 1;
                            dw_ext0 -= 1;
                            wsv_ext1 += 2;
                            dw_ext1 -= 2;
                        }
                        //One contribution is a permutation of (1,1,1,-1) based on c2
                        xsv_ext2 = xsb + 1;
                        ysv_ext2 = ysb + 1;
                        zsv_ext2 = zsb + 1;
                        wsv_ext2 = wsb + 1;
                        dx_ext2 = dx0 - 1 - 2 * SQUISH_CONSTANT_4D;
                        dy_ext2 = dy0 - 1 - 2 * SQUISH_CONSTANT_4D;
                        dz_ext2 = dz0 - 1 - 2 * SQUISH_CONSTANT_4D;
                        dw_ext2 = dw0 - 1 - 2 * SQUISH_CONSTANT_4D;
                        if ((c2 & 0x1) == 0) 
                        {
                            xsv_ext2 -= 2;
                            dx_ext2 += 2;
                        }
                        else if ((c2 & 0x2) == 0) 
                        {
                            ysv_ext2 -= 2;
                            dy_ext2 += 2;
                        }
                        else if ((c2 & 0x4) == 0) 
                        {
                            zsv_ext2 -= 2;
                            dz_ext2 += 2;
                        }
                        else 
                        {
                            wsv_ext2 -= 2;
                            dw_ext2 += 2;
                        }
                    } 
                    else //Both closest points on the smaller side
                    {
                        //One of the two extra points is (1,1,1,1)
                        xsv_ext2 = xsb + 1;
                        ysv_ext2 = ysb + 1;
                        zsv_ext2 = zsb + 1;
                        wsv_ext2 = wsb + 1;
                        dx_ext2 = dx0 - 1 - 4 * SQUISH_CONSTANT_4D;
                        dy_ext2 = dy0 - 1 - 4 * SQUISH_CONSTANT_4D;
                        dz_ext2 = dz0 - 1 - 4 * SQUISH_CONSTANT_4D;
                        dw_ext2 = dw0 - 1 - 4 * SQUISH_CONSTANT_4D;
                        //Other two points are based on the shared axes.
                        byte c = (byte)(aPoint & bPoint);
                        if ((c & 0x1) != 0) 
                        {
                            xsv_ext0 = xsb + 2;
                            xsv_ext1 = xsb + 1;
                            dx_ext0 = dx0 - 2 - 3 * SQUISH_CONSTANT_4D;
                            dx_ext1 = dx0 - 1 - 3 * SQUISH_CONSTANT_4D;
                        } 
                        else 
                        {
                            xsv_ext0 = xsb;
                            xsv_ext1 = xsb;
                            dx_ext0 = dx0 - 3 * SQUISH_CONSTANT_4D;
                            dx_ext1 = dx0 - 3 * SQUISH_CONSTANT_4D;
                        }
                        if ((c & 0x2) != 0) 
                        {
                            ysv_ext0 = ysb + 1;
                            ysv_ext1 = ysb + 1;
                            dy_ext0 = dy0 - 1 - 3 * SQUISH_CONSTANT_4D;
                            dy_ext1 = dy0 - 1 - 3 * SQUISH_CONSTANT_4D;
                            if ((c & 0x1) == 0) 
                            {
                                ysv_ext0 += 1;
                                dy_ext0 -= 1;
                            }
                            else 
                            {
                                ysv_ext1 += 1;
                                dy_ext1 -= 1;
                            }
                        } else {
                            ysv_ext0 = ysb;
                            ysv_ext1 = ysb;
                            dy_ext0 = dy0 - 3 * SQUISH_CONSTANT_4D;
                            dy_ext1 = dy0 - 3 * SQUISH_CONSTANT_4D;
                        }
                        if ((c & 0x4) != 0) 
                        {
                            zsv_ext0 = zsb + 1;
                            zsv_ext1 = zsb + 1;
                            dz_ext0 = dz0 - 1 - 3 * SQUISH_CONSTANT_4D;
                            dz_ext1 = dz0 - 1 - 3 * SQUISH_CONSTANT_4D;
                            if ((c & 0x3) == 0) 
                            {
                                zsv_ext0 += 1;
                                dz_ext0 -= 1;
                            }
                            else 
                            {
                                zsv_ext1 += 1;
                                dz_ext1 -= 1;
                            }
                        } 
                        else 
                        {
                            zsv_ext0 = zsb;
                            zsv_ext1 = zsb;
                            dz_ext0 = dz0 - 3 * SQUISH_CONSTANT_4D;
                            dz_ext1 = dz0 - 3 * SQUISH_CONSTANT_4D;
                        }
                        if ((c & 0x8) != 0) 
                        {
                            wsv_ext0 = wsb + 1;
                            wsv_ext1 = wsb + 2;
                            dw_ext0 = dw0 - 1 - 3 * SQUISH_CONSTANT_4D;
                            dw_ext1 = dw0 - 2 - 3 * SQUISH_CONSTANT_4D;
                        }
                        else 
                        {
                            wsv_ext0 = wsb;
                            wsv_ext1 = wsb;
                            dw_ext0 = dw0 - 3 * SQUISH_CONSTANT_4D;
                            dw_ext1 = dw0 - 3 * SQUISH_CONSTANT_4D;
                        }
                    }
                } 
                else //One point on each "side"
                {  
                    byte c1, c2;
                    if (aIsBiggerSide) 
                    {
                        c1 = aPoint;
                        c2 = bPoint;
                    }
                    else 
                    {
                        c1 = bPoint;
                        c2 = aPoint;
                    }
                    //Two contributions are the bigger-sided point with each 1 replaced with 2.
                    if ((c1 & 0x1) != 0) 
                    {
                        xsv_ext0 = xsb + 2;
                        xsv_ext1 = xsb + 1;
                        dx_ext0 = dx0 - 2 - 3 * SQUISH_CONSTANT_4D;
                        dx_ext1 = dx0 - 1 - 3 * SQUISH_CONSTANT_4D;
                    }
                    else 
                    {
                        xsv_ext0 = xsb;
                        xsv_ext1 = xsb;
                        dx_ext0 = dx0 - 3 * SQUISH_CONSTANT_4D;
                        dx_ext1 = dx0 - 3 * SQUISH_CONSTANT_4D;
                    }
                    if ((c1 & 0x2) != 0) 
                    {
                        ysv_ext0 = ysb + 1;
                        ysv_ext1 = ysb + 1;
                        dy_ext0 = dy0 - 1 - 3 * SQUISH_CONSTANT_4D;
                        dy_ext1 = dy0 - 1 - 3 * SQUISH_CONSTANT_4D;
                        if ((c1 & 0x1) == 0) 
                        {
                            ysv_ext0 += 1;
                            dy_ext0 -= 1;
                        }
                        else 
                        {
                            ysv_ext1 += 1;
                            dy_ext1 -= 1;
                        }
                    } 
                    else 
                    {
                        ysv_ext0 = ysb;
                        ysv_ext1 = ysb;
                        dy_ext0 = dy0 - 3 * SQUISH_CONSTANT_4D;
                        dy_ext1 = dy0 - 3 * SQUISH_CONSTANT_4D;
                    }
                    if ((c1 & 0x4) != 0) 
                    {
                        zsv_ext0 = zsb + 1;
                        zsv_ext1 = zsb + 1;
                        dz_ext0 = dz0 - 1 - 3 * SQUISH_CONSTANT_4D;
                        dz_ext1 = dz0 - 1 - 3 * SQUISH_CONSTANT_4D;
                        if ((c1 & 0x3) == 0) 
                        {
                            zsv_ext0 += 1;
                            dz_ext0 -= 1;
                        }
                        else 
                        {
                            zsv_ext1 += 1;
                            dz_ext1 -= 1;
                        }
                    } 
                    else 
                    {
                        zsv_ext0 = zsb;
                        zsv_ext1 = zsb;
                        dz_ext0 = dz0 - 3 * SQUISH_CONSTANT_4D;
                        dz_ext1 = dz0 - 3 * SQUISH_CONSTANT_4D;
                    }
                    if ((c1 & 0x8) != 0) 
                    {
                        wsv_ext0 = wsb + 1;
                        wsv_ext1 = wsb + 2;
                        dw_ext0 = dw0 - 1 - 3 * SQUISH_CONSTANT_4D;
                        dw_ext1 = dw0 - 2 - 3 * SQUISH_CONSTANT_4D;
                    }
                    else 
                    {
                        wsv_ext0 = wsb;
                        wsv_ext1 = wsb;
                        dw_ext0 = dw0 - 3 * SQUISH_CONSTANT_4D;
                        dw_ext1 = dw0 - 3 * SQUISH_CONSTANT_4D;
                    }
                    //One contribution is a permutation of (1,1,1,-1) based on the smaller-sided point
                    xsv_ext2 = xsb + 1;
                    ysv_ext2 = ysb + 1;
                    zsv_ext2 = zsb + 1;
                    wsv_ext2 = wsb + 1;
                    dx_ext2 = dx0 - 1 - 2 * SQUISH_CONSTANT_4D;
                    dy_ext2 = dy0 - 1 - 2 * SQUISH_CONSTANT_4D;
                    dz_ext2 = dz0 - 1 - 2 * SQUISH_CONSTANT_4D;
                    dw_ext2 = dw0 - 1 - 2 * SQUISH_CONSTANT_4D;
                    if ((c2 & 0x1) == 0) 
                    {
                        xsv_ext2 -= 2;
                        dx_ext2 += 2;
                    }
                    else if ((c2 & 0x2) == 0) 
                    {
                        ysv_ext2 -= 2;
                        dy_ext2 += 2;
                    }
                    else if ((c2 & 0x4) == 0) 
                    {
                        zsv_ext2 -= 2;
                        dz_ext2 += 2;
                    }
                    else 
                    {
                        wsv_ext2 -= 2;
                        dw_ext2 += 2;
                    }
                }
                //Contribution (1,1,1,0)
                double dx4 = dx0 - 1 - 3 * SQUISH_CONSTANT_4D;
                double dy4 = dy0 - 1 - 3 * SQUISH_CONSTANT_4D;
                double dz4 = dz0 - 1 - 3 * SQUISH_CONSTANT_4D;
                double dw4 = dw0 - 3 * SQUISH_CONSTANT_4D;
                double attn4 = 2 - dx4 * dx4 - dy4 * dy4 - dz4 * dz4 - dw4 * dw4;
                if (attn4 > 0) 
                {
                    attn4 *= attn4;
                    value += attn4 * attn4 * Extrapolate(xsb + 1, ysb + 1, zsb + 1, wsb + 0, dx4, dy4, dz4, dw4);
                }
                //Contribution (1,1,0,1)
                double dx3 = dx4;
                double dy3 = dy4;
                double dz3 = dz0 - 3 * SQUISH_CONSTANT_4D;
                double dw3 = dw0 - 1 - 3 * SQUISH_CONSTANT_4D;
                double attn3 = 2 - dx3 * dx3 - dy3 * dy3 - dz3 * dz3 - dw3 * dw3;
                if (attn3 > 0) 
                {
                    attn3 *= attn3;
                    value += attn3 * attn3 * Extrapolate(xsb + 1, ysb + 1, zsb + 0, wsb + 1, dx3, dy3, dz3, dw3);
                }
                //Contribution (1,0,1,1)
                double dx2 = dx4;
                double dy2 = dy0 - 3 * SQUISH_CONSTANT_4D;
                double dz2 = dz4;
                double dw2 = dw3;
                double attn2 = 2 - dx2 * dx2 - dy2 * dy2 - dz2 * dz2 - dw2 * dw2;
                if (attn2 > 0) 
                {
                    attn2 *= attn2;
                    value += attn2 * attn2 * Extrapolate(xsb + 1, ysb + 0, zsb + 1, wsb + 1, dx2, dy2, dz2, dw2);
                }
                //Contribution (0,1,1,1)
                double dx1 = dx0 - 3 * SQUISH_CONSTANT_4D;
                double dz1 = dz4;
                double dy1 = dy4;
                double dw1 = dw3;
                double attn1 = 2 - dx1 * dx1 - dy1 * dy1 - dz1 * dz1 - dw1 * dw1;
                if (attn1 > 0)
                {
                    attn1 *= attn1;
                    value += attn1 * attn1 * Extrapolate(xsb + 0, ysb + 1, zsb + 1, wsb + 1, dx1, dy1, dz1, dw1);
                }
                //Contribution (1,1,0,0)
                double dx5 = dx0 - 1 - 2 * SQUISH_CONSTANT_4D;
                double dy5 = dy0 - 1 - 2 * SQUISH_CONSTANT_4D;
                double dz5 = dz0 - 0 - 2 * SQUISH_CONSTANT_4D;
                double dw5 = dw0 - 0 - 2 * SQUISH_CONSTANT_4D;
                double attn5 = 2 - dx5 * dx5 - dy5 * dy5 - dz5 * dz5 - dw5 * dw5;
                if (attn5 > 0) 
                {
                    attn5 *= attn5;
                    value += attn5 * attn5 * Extrapolate(xsb + 1, ysb + 1, zsb + 0, wsb + 0, dx5, dy5, dz5, dw5);
                }
                //Contribution (1,0,1,0)
                double dx6 = dx0 - 1 - 2 * SQUISH_CONSTANT_4D;
                double dy6 = dy0 - 0 - 2 * SQUISH_CONSTANT_4D;
                double dz6 = dz0 - 1 - 2 * SQUISH_CONSTANT_4D;
                double dw6 = dw0 - 0 - 2 * SQUISH_CONSTANT_4D;
                double attn6 = 2 - dx6 * dx6 - dy6 * dy6 - dz6 * dz6 - dw6 * dw6;
                if (attn6 > 0)
                {
                    attn6 *= attn6;
                    value += attn6 * attn6 * Extrapolate(xsb + 1, ysb + 0, zsb + 1, wsb + 0, dx6, dy6, dz6, dw6);
                }

                //Contribution (1,0,0,1)
                double dx7 = dx0 - 1 - 2 * SQUISH_CONSTANT_4D;
                double dy7 = dy0 - 0 - 2 * SQUISH_CONSTANT_4D;
                double dz7 = dz0 - 0 - 2 * SQUISH_CONSTANT_4D;
                double dw7 = dw0 - 1 - 2 * SQUISH_CONSTANT_4D;
                double attn7 = 2 - dx7 * dx7 - dy7 * dy7 - dz7 * dz7 - dw7 * dw7;
                if (attn7 > 0) 
                {
                    attn7 *= attn7;
                    value += attn7 * attn7 * Extrapolate(xsb + 1, ysb + 0, zsb + 0, wsb + 1, dx7, dy7, dz7, dw7);
                }
                //Contribution (0,1,1,0)
                double dx8 = dx0 - 0 - 2 * SQUISH_CONSTANT_4D;
                double dy8 = dy0 - 1 - 2 * SQUISH_CONSTANT_4D;
                double dz8 = dz0 - 1 - 2 * SQUISH_CONSTANT_4D;
                double dw8 = dw0 - 0 - 2 * SQUISH_CONSTANT_4D;
                double attn8 = 2 - dx8 * dx8 - dy8 * dy8 - dz8 * dz8 - dw8 * dw8;
                if (attn8 > 0) 
                {
                    attn8 *= attn8;
                    value += attn8 * attn8 * Extrapolate(xsb + 0, ysb + 1, zsb + 1, wsb + 0, dx8, dy8, dz8, dw8);
                }
                //Contribution (0,1,0,1)
                double dx9 = dx0 - 0 - 2 * SQUISH_CONSTANT_4D;
                double dy9 = dy0 - 1 - 2 * SQUISH_CONSTANT_4D;
                double dz9 = dz0 - 0 - 2 * SQUISH_CONSTANT_4D;
                double dw9 = dw0 - 1 - 2 * SQUISH_CONSTANT_4D;
                double attn9 = 2 - dx9 * dx9 - dy9 * dy9 - dz9 * dz9 - dw9 * dw9;
                if (attn9 > 0) 
                {
                    attn9 *= attn9;
                    value += attn9 * attn9 * Extrapolate(xsb + 0, ysb + 1, zsb + 0, wsb + 1, dx9, dy9, dz9, dw9);
                }
                //Contribution (0,0,1,1)
                double dx10 = dx0 - 0 - 2 * SQUISH_CONSTANT_4D;
                double dy10 = dy0 - 0 - 2 * SQUISH_CONSTANT_4D;
                double dz10 = dz0 - 1 - 2 * SQUISH_CONSTANT_4D;
                double dw10 = dw0 - 1 - 2 * SQUISH_CONSTANT_4D;
                double attn10 = 2 - dx10 * dx10 - dy10 * dy10 - dz10 * dz10 - dw10 * dw10;
                if (attn10 > 0)
                {
                    attn10 *= attn10;
                    value += attn10 * attn10 * Extrapolate(xsb + 0, ysb + 0, zsb + 1, wsb + 1, dx10, dy10, dz10, dw10);
                }
            }
            //First extra vertex
            double attn_ext0 = 2 - dx_ext0 * dx_ext0 - dy_ext0 * dy_ext0 - dz_ext0 * dz_ext0 - dw_ext0 * dw_ext0;
            if(attn_ext0 > 0)
            {
                attn_ext0 *= attn_ext0;
                value += attn_ext0 * attn_ext0;
            }
            //Second extra vertex
            double attn_ext1 = 2 - dx_ext1 * dx_ext1 - dy_ext1 * dy_ext1 - dz_ext1 * dz_ext1 - dw_ext1 * dw_ext1;
            if(attn_ext1 > 0) 
            {
                attn_ext1 *= attn_ext1;
                value += attn_ext1 * attn_ext1;
            }
            //Third extra vertex
            double attn_ext2 = 2 - dx_ext2 * dx_ext2 - dy_ext2 * dy_ext2 - dz_ext2 * dz_ext2 - dw_ext2 * dw_ext2;
            if(attn_ext2 > 0) 
            {
                attn_ext2 *= attn_ext2;
                value += attn_ext2 * attn_ext2;
            }
            return (value / NORM_CONSTANT_4D + 1) / 2;
        }

        /// <summary>
        /// Calculates a 4D OpenSimplex Noise value between 0 and 1 for input values x,y,z,w with given octaves and persistence.
        /// </summary>
        /// <param name="x">The x input of the 4D OpenSimplex Noise function.</param>
        /// <param name="y">The y input of the 4D OpenSimplex Noise function.</param>
        /// <param name="z">The z input of the 4D OpenSimplex Noise function.</param>
        /// <param name="w">The w input of the 4D OpenSimplex Noise function.</param>
        /// <param name="octaves">Number of octaves used to calculate the 4D OpenSimplex Noise.</param>
        /// <param name="persistence">Relative strength of higher octaves in the 4D OpenSimplex Noise function.</param>
        public static double Noise4D(double x, double y, double z, double w, int octaves, double persistence)
        {
            double total = 0;
            double frequency = 1;
            double amplitude = 1;
            double maxValue = 0;  //Used for normalizing result to 0.0 - 1.0
            if (octaves <= 0) { return 0.0; }
            if (octaves == 1) { return Noise4D(x, y, z, w); }
            for (int I = 0; I < octaves; I++)
            {
                total += Noise4D(x * frequency, y * frequency, z * frequency, w * frequency) * amplitude;
                maxValue += amplitude;
                amplitude *= persistence;
                frequency *= 2;
            }
            return total / maxValue;
        }

        #endregion

        #region "Private"

        /// <summary>
        /// Gradients for 2D. They approximate the directions to the vertices of an octagon from the center.
        /// </summary>
        private static readonly sbyte[] Gradients2D = { 5, 2, 2, 5, -5, 2, -2, 5, 5, -2, 2, -5, -5, -2, -2, -5 };

        /// <summary>
        /// Gradients for 3D. They approximate the directions to the vertices of a rhombicuboctahedron
        /// <para>from the center, skewed so that the triangular and square facets </para>
        /// <para>can be inscribed inside circles of the same radius.</para>
        /// </summary>
        private static readonly sbyte[] Gradients3D = { -11,  4,  4, -4,  11,  4, -4,  4,  11,
                                                         11,  4,  4,  4,  11,  4,  4,  4,  11,
                                                        -11, -4,  4, -4, -11,  4, -4, -4,  11,
                                                         11, -4,  4,  4, -11,  4,  4, -4,  11,
                                                        -11,  4, -4, -4,  11, -4, -4,  4, -11,
                                                         11,  4, -4,  4,  11, -4,  4,  4, -11,
                                                        -11, -4, -4, -4, -11, -4, -4, -4, -11,
                                                         11, -4, -4,  4, -11, -4,  4, -4, -11 };

        /// <summary>
        /// Gradients for 4D. They approximate the directions to the vertices of a disprismatotesseractihexadecachoron
        /// <para>from the center, skewed so that the tetrahedral and cubic facets </para>
        /// <para>can be inscribed inside spheres of the same radius.</para>
        /// </summary>
        private static readonly sbyte[] Gradients4D = { 3,  1,  1,  1,  1,  3,  1,  1,  1,  1,  3,  1,  1,  1,  1,  3,
                                                       -3,  1,  1,  1, -1,  3,  1,  1, -1,  1,  3,  1, -1,  1,  1,  3,
                                                        3, -1,  1,  1,  1, -3,  1,  1,  1, -1,  3,  1,  1, -1,  1,  3,
                                                       -3, -1,  1,  1, -1, -3,  1,  1, -1, -1,  3,  1, -1, -1,  1,  3,
                                                        3,  1, -1,  1,  1,  3, -1,  1,  1,  1, -3,  1,  1,  1, -1,  3,
                                                       -3,  1, -1,  1, -1,  3, -1,  1, -1,  1, -3,  1, -1,  1, -1,  3,
                                                        3, -1, -1,  1,  1, -3, -1,  1,  1, -1, -3,  1,  1, -1, -1,  3,
                                                       -3, -1, -1,  1, -1, -3, -1,  1, -1, -1, -3,  1, -1, -1, -1,  3,
                                                        3,  1,  1, -1,  1,  3,  1, -1,  1,  1,  3, -1,  1,  1,  1, -3,
                                                       -3,  1,  1, -1, -1,  3,  1, -1, -1,  1,  3, -1, -1,  1,  1, -3,
                                                        3, -1,  1, -1,  1, -3,  1, -1,  1, -1,  3, -1,  1, -1,  1, -3,
                                                       -3, -1,  1, -1, -1, -3,  1, -1, -1, -1,  3, -1, -1, -1,  1, -3,
                                                        3,  1, -1, -1,  1,  3, -1, -1,  1,  1, -3, -1,  1,  1, -1, -3,
                                                       -3,  1, -1, -1, -1,  3, -1, -1, -1,  1, -3, -1, -1,  1, -1, -3,
                                                        3, -1, -1, -1,  1, -3, -1, -1,  1, -1, -3, -1,  1, -1, -1, -3,
                                                       -3, -1, -1, -1, -1, -3, -1, -1, -1, -1, -3, -1, -1, -1, -1, -3 };


        private static double Extrapolate(int xsb, int ysb, double dx, double dy)
        {
            int index = perm[(perm[xsb & 0xFF] + ysb) & 0xFF] & 0xE;
            return Gradients2D[index] * dx + Gradients2D[index + 1] * dy;
        }

        private static double Extrapolate(int xsb, int ysb, int zsb, double dx, double dy, double dz)
        {

            int index = permGradIndex3D[(perm[(perm[xsb & 0xFF] + ysb) & 0xFF] + zsb) & 0xFF];
            return Gradients3D[index] * dx + Gradients3D[index + 1] * dy + Gradients3D[index + 2] * dz;
        }

        private static double Extrapolate(int xsb, int ysb, int zsb, int wsb, double dx, double dy, double dz, double dw)
        {
            int index = perm[(perm[(perm[(perm[xsb & 0xFF] + ysb) & 0xFF] + zsb) & 0xFF] + wsb) & 0xFF] & 0xFC;
            return Gradients4D[index] * dx + Gradients4D[index + 1] * dy + Gradients4D[index + 2] * dz + Gradients4D[index + 3] * dw;
        }

        // This method is faster than using (int)Math.floor(x)
        private static int FastFloor(double x)
        {
            int xi = (int)x;
            return x < xi ? xi - 1 : xi;
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

