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
    internal class Box
    {
        public Rectangle Rect;
        private double Width;
        private double Height;
        public Vector Location;
        private double Angle;
        public bool alive;
        private Brush myFillColor;
        public B2BodyId my_ID;
        private RotateTransform Rot;

        public Box(Vector location, Size size, bool IsStatic, B2WorldId worldId)
        {
            Location = location;
            Width = size.Width;
            Height = size.Height;
            Angle = 0;
            Rect = new Rectangle()
            {
                Width = size.Width,
                Height = size.Height,
                Stroke = Brushes.Black,
                StrokeThickness = 2.0,
                Fill = Brushes.White,
            };
            Rect.SetValue(Canvas.LeftProperty, Location.X - size.Width / 2);
            Rect.SetValue(Canvas.TopProperty, Location.Y - size.Height / 2);
            Rot = new RotateTransform();
            Rect.RenderTransform = Rot;
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
            //bdef.linearVelocity = new B2Vec2(Utilities.P2W(100 * Rnd.NextDouble() - 50), Utilities.P2W(100 * Rnd.NextDouble() - 50));
            //bdef.angularVelocity = (float)Rnd.NextDouble();
            my_ID = B2Bodies.b2CreateBody(worldId, bdef);
            //Create a Box2D box shape
            B2Polygon box = B2Geometries.b2MakeBox(Utilities.P2W(size.Width / 2), Utilities.P2W(size.Height / 2));
            B2ShapeDef shapeDef = B2Types.b2DefaultShapeDef();
            shapeDef.density = 2.0f;
            shapeDef.material.restitution = 0.2f;
            shapeDef.material.friction = 0.5f;
            //Connect the Box2D box shape to the Box2D dynamic body
            B2Shapes.b2CreatePolygonShape(my_ID, shapeDef, box);
        }

        public Box(double width, double height) 
        {
            Location = new Vector(0, 0);
            Width = width;
            Height = height;
            Angle = 0;
            Rect = new Rectangle()
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
                Rect.Fill = myFillColor;
            }
        }

        public void Update()
        {
            //Get the new Location and Angle from the Box2D body
            B2Vec2 position = B2Bodies.b2Body_GetPosition(my_ID);
            B2Rot rotation = B2Bodies.b2Body_GetRotation(my_ID);
            Location = Utilities.Vec2Vector(position);
            Angle = B2MathFunction.b2Rot_GetAngle(rotation) * 180 / Math.PI;
            //Update the Box location 
            Rect.SetValue(Canvas.LeftProperty, Location.X - Width / 2);
            Rect.SetValue(Canvas.TopProperty, Location.Y - Height / 2);
            //Set the box rotation
            Rot.CenterX = Width / 2;
            Rot.CenterY = Height / 2;
            Rot.Angle = Angle;
        }

        //Boxes that fall off the edges are removed
        public void Edges(Canvas c)
        {
            alive = true;
            if (Location.X + Width < 0) { alive = false; }
            //if (Location.Y + Height < 0) { alive = false; }
            if (Location.X - Width > c.ActualWidth) { alive = false; }
            //if (Location.Y - Height > c.ActualHeight) { alive = false; }
        }

        public void Draw(Canvas c)
        {
            c.Children.Add(Rect);
        }

        public void Remove(Canvas c)
        {
            if (c.Children.Contains(Rect))
            {
                c.Children.Remove(Rect);
            }
            if (B2Worlds.b2Body_IsValid(my_ID) == true)
            {
                B2Bodies.b2DestroyBody(my_ID);
            }
        }
    }
}
