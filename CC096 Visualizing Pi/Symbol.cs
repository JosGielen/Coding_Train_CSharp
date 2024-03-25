using System.Globalization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Visualizing_Pi
{
    internal class Symbol : Shape
    {
        private Point my_Origin;
        private string my_Text;
        private Typeface face;
        private FormattedText ftext;
        private Color my_Color;
        private FontFamily my_FontFamily;
        private int my_FontSize;
        private FontStyle my_FontStyle;
        private FontWeight My_FontWeight;
        //private static Random Rnd = new Random();

        public Symbol(string text, Point origin, Color color, string fontFamilyName, int fontSize)
        {
            my_Text = text;
            my_Origin = origin;
            my_Color = color;
            my_FontFamily = new FontFamily(fontFamilyName);
            my_FontSize = fontSize;
            my_FontStyle = FontStyles.Normal;
            My_FontWeight = FontWeights.Normal;
            SetFText();
}

        public string Text
        {
            get { return my_Text; }
            set
            {
                my_Text = value;
                SetFText();
            }
        }

        public Color TxtColor
        {
            get { return my_Color; }
            set { my_Color = value; }
        }

        public double Left
        {
            get { return my_Origin.X; }
            set { my_Origin.X = value; }
        }

        public double Top
        {
            get { return my_Origin.Y; }
            set { my_Origin.Y = value; }
        }

        public FontFamily FontFamily
        {
            get { return my_FontFamily; }
            set
            {
                my_FontFamily = value;
                SetFText();
            }
        }

        public int FontSize
        {
            get { return my_FontSize; }
            set
            {
                my_FontSize = value;
                SetFText();
            }
        }

        public FontStyle FontStyle
        {
            get { return my_FontStyle; }
            set
            {
                my_FontStyle = value;
                SetFText();
            }
        }

        public FontWeight FontWeight
        {
            get { return My_FontWeight; }
            set
            {
                My_FontWeight = value;
                SetFText();
            }
        }

        public Geometry Geometry
        {
            get { return ftext.BuildGeometry(my_Origin); }
        }

        private void SetFText()
        {
            face = new Typeface(my_FontFamily, my_FontStyle, My_FontWeight, FontStretches.Normal);
            ftext = new FormattedText(my_Text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, face, my_FontSize * 96.0 / 72.0, new SolidColorBrush(my_Color), VisualTreeHelper.GetDpi(this).PixelsPerDip);
            Height = ftext.Height;
            Width = ftext.Width;
            InvalidateVisual();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            drawingContext.DrawText(ftext, my_Origin);
        }

        protected override Geometry DefiningGeometry
        {
            get { return ftext.BuildGeometry(my_Origin); }
        }
    }
}
