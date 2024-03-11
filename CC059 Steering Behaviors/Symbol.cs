using System.Windows.Media;
using System.Windows;
using System.Windows.Shapes;

namespace Steering_Behaviors
{
    class Symbol : Shape
    {
        private Point my_Origin;
        private string my_Text;
        private Typeface face;
        private FormattedText ftext;
        private FontFamily my_FontFamily;
        private double my_FontSize;
        private FontStyle my_FontStyle;
        private FontWeight My_FontWeight;
        private Geometry my_Geometry;

        public Symbol(string text)
        {
            my_Text = text;
            my_Origin = new Point(0, 0);
            my_FontFamily = new FontFamily("Arial");
            my_FontSize = 12;
            my_FontStyle = FontStyles.Normal;
            My_FontWeight = FontWeights.Bold;
            Update();
        }
        public string Text
        {
            get { return my_Text; }
            set
            {
                my_Text = value;
                Update();
            }
        }

        public Point Origin
        {
            get { return my_Origin; }
            set
            {
                my_Origin = value;
                Update();
            }
        }

        public FontFamily FontFamily
        {
            get { return my_FontFamily; }
            set
            {
                my_FontFamily = value;
                Update();
            }
        }

        public double FontSize
        {
            get { return my_FontSize; }
            set
            {
                my_FontSize = value;
                Update();
            }
        }

        public FontStyle FontStyle
        {
            get { return my_FontStyle; }
            set
            {
                my_FontStyle = value;
                Update();
            }
        }

        public FontWeight FontWeight
        {
            get { return My_FontWeight; }
            set
            {
                My_FontWeight = value;
                Update();
            }
        }

        public Geometry Geometry
        {
            get { return ftext.BuildGeometry(my_Origin); }
        }

        private void Update()
        {
            face = new Typeface(my_FontFamily, my_FontStyle, My_FontWeight, FontStretches.Normal);
            ftext = new FormattedText(my_Text, new System.Globalization.CultureInfo("en-GB"), FlowDirection.LeftToRight, face, my_FontSize * 96.0 / 72.0, Fill);
            my_Geometry = ftext.BuildGeometry(my_Origin);
        }

        protected override Geometry DefiningGeometry
        {
            get { return my_Geometry; }
        }

        protected override Size MeasureOverride(Size constraint)
        {
            if (my_Geometry.Bounds == Rect.Empty)
            {
                return new Size(0, 0);
            }
            else
            {
                return new Size(Math.Min(constraint.Width, my_Geometry.Bounds.Width), Math.Min(constraint.Height, my_Geometry.Bounds.Height));
            }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            drawingContext.DrawGeometry(Fill, new Pen(Stroke, StrokeThickness), my_Geometry);
        }

    }
}
