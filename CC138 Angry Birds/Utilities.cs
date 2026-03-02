using Box2D.NET;
using System.Windows;

namespace Angry_Birds
{
    internal class Utilities
    {
        private static double P2Wscale = 0.05;  //example 20 pixels = 1 meter
        private static double W2Pscale = 20;


        /// <summary>
        /// Convert double type Pixel values into float type Box2D World values
        /// </summary>
        /// <param name="A">Pixel value</param>
        /// <returns></returns>
        public static float P2W(double A)
        {
            return (float)(P2Wscale * A);
        }

        /// <summary>
        /// Convert float type Box2D World values into double type Pixel values
        /// </summary>
        /// <param name="A">Box2D World value</param>
        /// <returns></returns>
        public static double W2P(float A)
        {
            return (double)(W2Pscale * A);
        }

        /// <summary>
        /// Convert Vector type Pixel coördinates into B2Vec2 type Box2D World values
        /// </summary>
        /// <param name="vec">Vector in pixel coördinates</param>
        /// <returns></returns>
        public static B2Vec2 Vector2Vec(Vector vec)
        {
            return new B2Vec2(P2W(vec.X), P2W(vec.Y));
        }

        /// <summary>
        /// Convert B2Vec2 type Box2D World coördinates into Vector type Pixel values
        /// </summary>
        /// <param name="vec">B2Vec2 in Box2D World coördinates</param>
        /// <returns></returns>
        public static Vector Vec2Vector(B2Vec2 vec)
        {
            return new Vector(W2P(vec.X), W2P(vec.Y));
        }
    }
}
