using System;
using System.Collections.Generic;
using Perlin_Noise;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Forward_Kinematics
{
    class Flagellum
    {
        private List<Segment> Segments;
        private Vector my_Base;
        private double my_StartAngle;
        private double my_Segmentlength;
        private static Random Rnd = new Random();
        private double Xoff = Rnd.Next(1000);

        public Flagellum(Vector base_, double angle, int segmentcount, double totallength, Brush color_)
        {
            Segments = new List<Segment>();
            Segment s;
            my_Base = base_;
            my_StartAngle = angle;
            my_Segmentlength = totallength / segmentcount;
            s = new Segment(base_.X, base_.Y, my_Segmentlength, my_StartAngle);
            s.LineColor = color_;
            s.LineThickness = 7;
            Segments.Add(s);
            for (int I = 1; I < segmentcount; I++)
            {
                s = new Segment(base_.X, base_.Y, my_Segmentlength, 0);
                s.LineColor = color_;
                s.LineThickness = 6 * (segmentcount - I) / (segmentcount - 1) + 1;
                Segments.Add(s);
            }
        }

        public Vector Base
        {
            get { return my_Base; }
            set { my_Base = value; }
        }

        public void Show(Canvas c)
        {
            for (int I = 0; I < Segments.Count; I++)
            {
                Segments[I].Show(c);
            }
        }

        public void Update(double Yoff)
        {
            Segments[0].Angle = my_StartAngle;
            Segments[0].StartPt = my_Base;

            for (int I = 1; I < Segments.Count; I++)
            {
                Segments[I].Angle = 20 * PerlinNoise.Noise(Xoff + Yoff, 2, 0.5) - 10;
            }
            Segments[0].Update();
            for (int I = 1; I < Segments.Count; I++)
            {
                Segments[I].Update(Segments[I - 1]);
            }
        }
    }
}
