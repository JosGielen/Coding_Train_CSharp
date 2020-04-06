using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media;

namespace BubbleSortVisualization
{
    class ColorPalette
    {

        private static List<Color> my_Colors;

        /// <summary>
        /// Creates a new empty ColorPalette. Use LoadPalette to read the colors from a .CPL file.
        /// </summary>
        public ColorPalette()
        {
            my_Colors = new List<Color>();
        }

        /// <summary>
        /// Creates a ColorPalette from a file.
        /// <para>Each line in the file must have R;G;B format (R, G, B = 0-255)</para>
        /// </summary>
        public ColorPalette(string file)
        {
            LoadPalette(file);
        }

        /// <summary>
        /// Read the color data from a file.
        /// <para>Each line in the file must have R;G;B format (R, G, B = 0-255).</para>
        /// </summary>
        public void LoadPalette(string file)
        {
            string Line;
            string[] txtParts;
            byte r;
            byte g;
            byte b;
            my_Colors = new List<Color>();
            StreamReader sr = null;
            try
            {
                sr = new StreamReader(file);
                while (!sr.EndOfStream)
                {
                    Line = sr.ReadLine();
                    txtParts = Line.Split(';');
                    r = byte.Parse(txtParts[0]);
                    g = byte.Parse(txtParts[1]);
                    b = byte.Parse(txtParts[2]);
                    my_Colors.Add(Color.FromRgb(r, g, b));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Cannot load the palette. Original error: " + ex.Message, "ColorPalette error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                if (sr != null)
                {
                    sr.Close();
                }
            }
        }

        public List<Color> GetColors(int size)
        {
            List<Color> Result = new List<Color>();
            int my_Size = my_Colors.Count;
            int newIndex = 0;
            double mappedIndex;
            for (int oldIndex = 0; oldIndex < my_Size; oldIndex++)
            {
                mappedIndex = oldIndex * size / my_Size;
                while (mappedIndex >= newIndex )
                {
                    Result.Add(my_Colors[oldIndex]);
                    newIndex++;
                }
            }
            return Result;
        }

        public List<Brush> GetColorBrushes(int size)
        {
            List<Color> tmpColors = GetColors(size);
            List<Brush> result = new List<Brush>();
            for (int I = 0; I < tmpColors.Count; I++)
            {
                result.Add(new SolidColorBrush(tmpColors[I]));
            }
            return result;
        }
    }
}
