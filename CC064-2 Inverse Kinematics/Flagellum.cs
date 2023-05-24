using System;
using System.Collections.Generic;
using Color_Palette;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Inverse_Kinematics
{
    class Flagellum
    {
        private List<Segment> Segments;
        private List<Brush> My_Colors;

        public Flagellum(int segmentcount, double totallength)
        {
            Segments = new List<Segment>();
            My_Colors = new List<Brush>();
            ColorPalette pal = new ColorPalette(Environment.CurrentDirectory + "\\Rainbow.cpl");
            My_Colors = pal.GetColorBrushes(segmentcount);
            double Segmentlength = totallength / segmentcount;
            Segment s;
            for (int I = 0; I < segmentcount; I++)
            {
                s = new Segment(Segmentlength)
                {
                    LineColor = My_Colors[segmentcount - I - 1],
                    LineThickness = 6 * I / (segmentcount - 1) + 1
                };
                Segments.Add(s);
            }
        }

        public void Show(Canvas c)
        {
            for (int I = 0; I < Segments.Count; I++)
            {
                Segments[I].Show(c);
            }
        }

        public void Follow(Vector target)
        {
            for (int I = Segments.Count - 1; I >= 0; I--)
            {
                Segments[I].Follow(target);
                target = Segments[I].StartPt;
            }
        }
    }
}
