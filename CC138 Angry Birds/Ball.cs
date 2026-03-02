using Box2D.NET;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
//using static Box2D_Test.Utilities;
//using static Box2D.NET.B2Bodies;
//using static Box2D.NET.B2MathFunction;

namespace Angry_Birds
{
    internal class Ball
    {
        public Ellipse Ell;
        public double Radius;
        public Vector Location;
        public double Angle;
        public bool alive;
        public Brush myFillColor;
        public B2BodyId my_ID;
        private RotateTransform Rot;

        public Ball(Vector location, double radius, bool IsStatic, B2WorldId worldId)
        {
            Location = location;
            Radius = radius;
            Angle = 0;
            Ell = new Ellipse()
            {
                Width = 2 * radius,
                Height = 2 * radius,
                Stroke = Brushes.Black,
                StrokeThickness = 2.0,
                Fill = Brushes.White,
            };
            Ell.SetValue(Canvas.LeftProperty, Location.X - radius);
            Ell.SetValue(Canvas.TopProperty, Location.Y - radius);
            Rot = new RotateTransform();
            Ell.RenderTransform = Rot;
            //Create a Box2D body
            B2BodyDef bdef = B2Types.b2DefaultBodyDef();
            if (IsStatic)
            {
                bdef.type = B2BodyType.b2_staticBody;
            }
            else
            {
                bdef.type = B2BodyType.b2_dynamicBody;
            }
            bdef.position = Utilities.Vector2Vec(location);
            bdef.linearVelocity = new B2Vec2(0.0f, 0.0f);
            bdef.angularVelocity = 0.0f;
            my_ID = B2Bodies.b2CreateBody(worldId, bdef);
            //Create a Box2D Rounded box shape
            B2Polygon box = B2Geometries.b2MakeRoundedBox(Utilities.P2W(0.5 * radius), Utilities.P2W(0.5 * radius), Utilities.P2W(0.5 * radius));
            B2ShapeDef shapeDef = B2Types.b2DefaultShapeDef();
            shapeDef.density = 1.0f;
            shapeDef.material.restitution = 0.2f;
            shapeDef.material.friction = 0.2f;
            //Connect the Box2D box shape to the Box2D dynamic body
            B2Shapes.b2CreatePolygonShape(my_ID, shapeDef, box);
        }

        public Ball(double radius)
        {
            Location = new Vector(0, 0);
            Radius = radius;
            Angle = 0;
            Ell = new Ellipse()
            {
                Width = 0.0,
                Height = 0.0,
                Stroke = Brushes.Black,
                StrokeThickness = 2.0,
                Fill = Brushes.White,
            };
        }

        public Brush FillColor
        {
            get { return myFillColor; }
            set
            {
                myFillColor = value;
                Ell.Fill = myFillColor;
            }
        }

        public void Update()
        {
            if (B2Worlds.b2Body_IsValid(my_ID) == true)
            {
                //Get the new Location and Angle from the Box2D body
                B2Vec2 position = B2Bodies.b2Body_GetPosition(my_ID);
                B2Rot rotation = B2Bodies.b2Body_GetRotation(my_ID);
                Location = Utilities.Vec2Vector(position);
                Angle = B2MathFunction.b2Rot_GetAngle(rotation) * 180 / Math.PI;
                //Update the Ball location 
                Ell.SetValue(Canvas.LeftProperty, Location.X - Radius);
                Ell.SetValue(Canvas.TopProperty, Location.Y - Radius);
                //Set the Ball rotation
                Rot.CenterX = Radius;
                Rot.CenterY = Radius;
                Rot.Angle = Angle;
            }
        }

        //Balls that fall off the edges are removed
        public void Edges(Canvas c)
        {
            alive = true;
            if (Location.X + 2 * Radius < 0) { alive = false; }
            if (Location.Y + 2 * Radius < 0) { alive = false; }
            if (Location.X - 2 * Radius > c.ActualWidth) { alive = false; }
            if (Location.Y - 2 * Radius > c.ActualHeight) { alive = false; }
        }

        public void Draw(Canvas c)
        {
            c.Children.Add(Ell);
        }

        public void Remove(Canvas c)
        {
            if (c.Children.Contains(Ell))
            {
                c.Children.Remove(Ell);
            }
            if (B2Worlds.b2Body_IsValid(my_ID) == true)
            {
                B2Bodies.b2DestroyBody(my_ID);
            }
        }

    }
}
