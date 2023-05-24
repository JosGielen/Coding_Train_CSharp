using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Inverse_Kinematics_Multiple
{
    class Flagellum
    {
        private List<Segment> Segments;
        private Vector my_Base;
        private int my_SegmentCount;
        private double my_Segmentlength;

        public Flagellum(Vector base_, int segmentcount, double totallength, Brush color_)
        {
            Segments = new List<Segment>();
            Segment s;
            my_Base = base_;
            my_SegmentCount = segmentcount;
            my_Segmentlength = totallength / segmentcount;
            s = new Segment(base_.X, base_.Y, my_Segmentlength);
            s.LineColor = color_;
            s.LineThickness = 7;
            Segments.Add(s);
            for (int I = 0; I < segmentcount; I++)
            {
                s = new Segment(base_.X, base_.Y, my_Segmentlength);
                s.LineColor = color_;
                s.LineThickness = 5 * (segmentcount - I - 1) / (segmentcount - 1) + 2;
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
            Vector fixedPt = my_Base;
            for (int I = 0; I < Segments.Count; I++)
            {
                Segments[I].SetStart(fixedPt);
                fixedPt = Segments[I].EndPt;
            }
        }
    }
}
