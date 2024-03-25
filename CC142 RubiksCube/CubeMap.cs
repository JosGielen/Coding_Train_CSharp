using RubiksCube;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace RubiksCube
{
    internal class CubeMap
    {
        private List<CubeletFace> my_CubeletFaces;

        public CubeMap(Canvas c)
        {
            //Make the 2D CubeletFaces
            CubeletFace cf;
            double side; //The size of a cubeletFace
            double borderX;
            double borderY;
            double left;
            double top;
            bool Isfixed;
            //Determine the optimal size of a cubeletFace
            my_CubeletFaces = new List<CubeletFace>();
            if ((c.ActualWidth - 50) / 4 > (c.ActualHeight - 50) / 3)
            {
                side = (c.ActualHeight - 50) / 9;
                borderX = (c.ActualWidth - 12 * side) / 2;
                borderY = 25;
            }
            else
            {
                side = (c.ActualWidth - 50) / 12;
                borderX = 25;
                borderY = (c.ActualHeight - 9 * side) / 2;
            }
            c.Children.Clear();
            //Left Side (Orange)
            for (int I = 0; I <= 2; I++)
            {
                for (int J = 0; J <= 2; J++)
                {
                    Isfixed = (I == 1 & J == 1);
                    left = borderX + I * side;
                    top = borderY + (J + 3) * side;
                    cf = new CubeletFace(4, left, top, side, Isfixed);
                    my_CubeletFaces.Add(cf);
                    cf.Draw(c);
                }
            }
            //FRONT Side (Green)
            for (int I = 0; I <= 2; I++)
            {
                for (int J = 0; J <= 2; J++)
                {
                    Isfixed = (I == 1 & J == 1);
                    left = borderX + (I + 3) * side;
                    top = borderY + (J + 3) * side;
                    cf = new CubeletFace(2, left, top, side, Isfixed);
                    my_CubeletFaces.Add(cf);
                    cf.Draw(c);
                }
            }
            //RIGHT Side (Red)
            for (int I = 0; I <= 2; I++)
            {
                for (int J = 0; J <= 2; J++)
                {
                    Isfixed = (I == 1 & J == 1);
                    left = borderX + (I + 6) * side;
                    top = borderY + (J + 3) * side;
                    cf = new CubeletFace(5, left, top, side, Isfixed);
                    my_CubeletFaces.Add(cf);
                    cf.Draw(c);
                }
            }
            //BACK Side (Blue)
            for (int I = 0; I <= 2; I++)
            {
                for (int J = 0; J <= 2; J++)
                {
                    Isfixed = (I == 1 & J == 1);
                    left = borderX + (I + 9) * side;
                    top = borderY + (J + 3) * side;
                    cf = new CubeletFace(3, left, top, side, Isfixed);
                    my_CubeletFaces.Add(cf);
                    cf.Draw(c);
                }
            }
            //UP Side (White)
            for (int I = 0; I <= 2; I++)
            {
                for (int J = 0; J <= 2; J++)
                {
                    Isfixed = (I == 1 & J == 1);
                    left = borderX + (I + 3) * side;
                    top = borderY + J * side;
                    cf = new CubeletFace(0, left, top, side, Isfixed);
                    my_CubeletFaces.Add(cf);
                    cf.Draw(c);
                }
            }
            //DOWN Side (Yellow)
            for (int I = 0; I <= 2; I++)
            {
                for (int J = 0; J <= 2; J++)
                {
                    Isfixed = (I == 1 & J == 1);
                    left = borderX + (I + 3) * side;
                    top = borderY + (J + 6) * side;
                    cf = new CubeletFace(1, left, top, side, Isfixed);
                    my_CubeletFaces.Add(cf);
                    cf.Draw(c);
                }
            }
        }

        public List<CubeletFace> CubeletFaces
        {
            get { return my_CubeletFaces; }
        }

        #region "Rotations"

        public void RotateUP2D(bool ClockWise)
        {
            int temp;
            if (ClockWise)
            {
                for (int I = 0; I <= 1; I++)
                {
                    temp = CubeletFaces[36].FaceColorNumber;
                    CubeletFaces[36].FaceColorNumber = CubeletFaces[37].FaceColorNumber;
                    CubeletFaces[37].FaceColorNumber = CubeletFaces[38].FaceColorNumber;
                    CubeletFaces[38].FaceColorNumber = CubeletFaces[41].FaceColorNumber;
                    CubeletFaces[41].FaceColorNumber = CubeletFaces[44].FaceColorNumber;
                    CubeletFaces[44].FaceColorNumber = CubeletFaces[43].FaceColorNumber;
                    CubeletFaces[43].FaceColorNumber = CubeletFaces[42].FaceColorNumber;
                    CubeletFaces[42].FaceColorNumber = CubeletFaces[39].FaceColorNumber;
                    CubeletFaces[39].FaceColorNumber = temp;
                }
                for (int I = 0; I <= 2; I++)
                {
                    temp = CubeletFaces[0].FaceColorNumber;
                    CubeletFaces[0].FaceColorNumber = CubeletFaces[3].FaceColorNumber;
                    CubeletFaces[3].FaceColorNumber = CubeletFaces[6].FaceColorNumber;
                    CubeletFaces[6].FaceColorNumber = CubeletFaces[9].FaceColorNumber;
                    CubeletFaces[9].FaceColorNumber = CubeletFaces[12].FaceColorNumber;
                    CubeletFaces[12].FaceColorNumber = CubeletFaces[15].FaceColorNumber;
                    CubeletFaces[15].FaceColorNumber = CubeletFaces[18].FaceColorNumber;
                    CubeletFaces[18].FaceColorNumber = CubeletFaces[21].FaceColorNumber;
                    CubeletFaces[21].FaceColorNumber = CubeletFaces[24].FaceColorNumber;
                    CubeletFaces[24].FaceColorNumber = CubeletFaces[27].FaceColorNumber;
                    CubeletFaces[27].FaceColorNumber = CubeletFaces[30].FaceColorNumber;
                    CubeletFaces[30].FaceColorNumber = CubeletFaces[33].FaceColorNumber;
                    CubeletFaces[33].FaceColorNumber = temp;
                }
            }
            else
            {
                for (int I = 0; I <= 1; I++)
                {
                    temp = CubeletFaces[36].FaceColorNumber;
                    CubeletFaces[36].FaceColorNumber = CubeletFaces[39].FaceColorNumber;
                    CubeletFaces[39].FaceColorNumber = CubeletFaces[42].FaceColorNumber;
                    CubeletFaces[42].FaceColorNumber = CubeletFaces[43].FaceColorNumber;
                    CubeletFaces[43].FaceColorNumber = CubeletFaces[44].FaceColorNumber;
                    CubeletFaces[44].FaceColorNumber = CubeletFaces[41].FaceColorNumber;
                    CubeletFaces[41].FaceColorNumber = CubeletFaces[38].FaceColorNumber;
                    CubeletFaces[38].FaceColorNumber = CubeletFaces[37].FaceColorNumber;
                    CubeletFaces[37].FaceColorNumber = temp;
                }
                for (int I = 0; I <= 2; I++)
                {
                    temp = CubeletFaces[33].FaceColorNumber;
                    CubeletFaces[33].FaceColorNumber = CubeletFaces[30].FaceColorNumber;
                    CubeletFaces[30].FaceColorNumber = CubeletFaces[27].FaceColorNumber;
                    CubeletFaces[27].FaceColorNumber = CubeletFaces[24].FaceColorNumber;
                    CubeletFaces[24].FaceColorNumber = CubeletFaces[21].FaceColorNumber;
                    CubeletFaces[21].FaceColorNumber = CubeletFaces[18].FaceColorNumber;
                    CubeletFaces[18].FaceColorNumber = CubeletFaces[15].FaceColorNumber;
                    CubeletFaces[15].FaceColorNumber = CubeletFaces[12].FaceColorNumber;
                    CubeletFaces[12].FaceColorNumber = CubeletFaces[9].FaceColorNumber;
                    CubeletFaces[9].FaceColorNumber = CubeletFaces[6].FaceColorNumber;
                    CubeletFaces[6].FaceColorNumber = CubeletFaces[3].FaceColorNumber;
                    CubeletFaces[3].FaceColorNumber = CubeletFaces[0].FaceColorNumber;
                    CubeletFaces[0].FaceColorNumber = temp;
                }
            }
        }

        public void RotateDOWN2D(bool ClockWise)
        {
            int temp;
            if (ClockWise)
            {
                for (int I = 0; I <= 1; I++)
                {
                    temp = CubeletFaces[45].FaceColorNumber;
                    CubeletFaces[45].FaceColorNumber = CubeletFaces[46].FaceColorNumber;
                    CubeletFaces[46].FaceColorNumber = CubeletFaces[47].FaceColorNumber;
                    CubeletFaces[47].FaceColorNumber = CubeletFaces[50].FaceColorNumber;
                    CubeletFaces[50].FaceColorNumber = CubeletFaces[53].FaceColorNumber;
                    CubeletFaces[53].FaceColorNumber = CubeletFaces[52].FaceColorNumber;
                    CubeletFaces[52].FaceColorNumber = CubeletFaces[51].FaceColorNumber;
                    CubeletFaces[51].FaceColorNumber = CubeletFaces[48].FaceColorNumber;
                    CubeletFaces[48].FaceColorNumber = temp;
                }
                for (int I = 0; I <= 2; I++)
                {
                    temp = CubeletFaces[35].FaceColorNumber;
                    CubeletFaces[35].FaceColorNumber = CubeletFaces[32].FaceColorNumber;
                    CubeletFaces[32].FaceColorNumber = CubeletFaces[29].FaceColorNumber;
                    CubeletFaces[29].FaceColorNumber = CubeletFaces[26].FaceColorNumber;
                    CubeletFaces[26].FaceColorNumber = CubeletFaces[23].FaceColorNumber;
                    CubeletFaces[23].FaceColorNumber = CubeletFaces[20].FaceColorNumber;
                    CubeletFaces[20].FaceColorNumber = CubeletFaces[17].FaceColorNumber;
                    CubeletFaces[17].FaceColorNumber = CubeletFaces[14].FaceColorNumber;
                    CubeletFaces[14].FaceColorNumber = CubeletFaces[11].FaceColorNumber;
                    CubeletFaces[11].FaceColorNumber = CubeletFaces[8].FaceColorNumber;
                    CubeletFaces[8].FaceColorNumber = CubeletFaces[5].FaceColorNumber;
                    CubeletFaces[5].FaceColorNumber = CubeletFaces[2].FaceColorNumber;
                    CubeletFaces[2].FaceColorNumber = temp;
                }
            }
            else
            {
                for (int I = 0; I <= 1; I++)
                {
                    temp = CubeletFaces[45].FaceColorNumber;
                    CubeletFaces[45].FaceColorNumber = CubeletFaces[48].FaceColorNumber;
                    CubeletFaces[48].FaceColorNumber = CubeletFaces[51].FaceColorNumber;
                    CubeletFaces[51].FaceColorNumber = CubeletFaces[52].FaceColorNumber;
                    CubeletFaces[52].FaceColorNumber = CubeletFaces[53].FaceColorNumber;
                    CubeletFaces[53].FaceColorNumber = CubeletFaces[50].FaceColorNumber;
                    CubeletFaces[50].FaceColorNumber = CubeletFaces[47].FaceColorNumber;
                    CubeletFaces[47].FaceColorNumber = CubeletFaces[46].FaceColorNumber;
                    CubeletFaces[46].FaceColorNumber = temp;
                }
                for (int I = 0; I <= 2; I++)
                {
                    temp = CubeletFaces[2].FaceColorNumber;
                    CubeletFaces[2].FaceColorNumber = CubeletFaces[5].FaceColorNumber;
                    CubeletFaces[5].FaceColorNumber = CubeletFaces[8].FaceColorNumber;
                    CubeletFaces[8].FaceColorNumber = CubeletFaces[11].FaceColorNumber;
                    CubeletFaces[11].FaceColorNumber = CubeletFaces[14].FaceColorNumber;
                    CubeletFaces[14].FaceColorNumber = CubeletFaces[17].FaceColorNumber;
                    CubeletFaces[17].FaceColorNumber = CubeletFaces[20].FaceColorNumber;
                    CubeletFaces[20].FaceColorNumber = CubeletFaces[23].FaceColorNumber;
                    CubeletFaces[23].FaceColorNumber = CubeletFaces[26].FaceColorNumber;
                    CubeletFaces[26].FaceColorNumber = CubeletFaces[29].FaceColorNumber;
                    CubeletFaces[29].FaceColorNumber = CubeletFaces[32].FaceColorNumber;
                    CubeletFaces[32].FaceColorNumber = CubeletFaces[35].FaceColorNumber;
                    CubeletFaces[35].FaceColorNumber = temp;
                }
            }
        }

        public void RotateLEFT2D(bool ClockWise)
        {
            int temp;
            if (ClockWise)
            {
                for (int I = 0; I <= 1; I++)
                {
                    temp = CubeletFaces[0].FaceColorNumber;
                    CubeletFaces[0].FaceColorNumber = CubeletFaces[1].FaceColorNumber;
                    CubeletFaces[1].FaceColorNumber = CubeletFaces[2].FaceColorNumber;
                    CubeletFaces[2].FaceColorNumber = CubeletFaces[5].FaceColorNumber;
                    CubeletFaces[5].FaceColorNumber = CubeletFaces[8].FaceColorNumber;
                    CubeletFaces[8].FaceColorNumber = CubeletFaces[7].FaceColorNumber;
                    CubeletFaces[7].FaceColorNumber = CubeletFaces[6].FaceColorNumber;
                    CubeletFaces[6].FaceColorNumber = CubeletFaces[3].FaceColorNumber;
                    CubeletFaces[3].FaceColorNumber = temp;
                }
                for (int I = 0; I <= 2; I++)
                {
                    temp = CubeletFaces[47].FaceColorNumber;
                    CubeletFaces[47].FaceColorNumber = CubeletFaces[46].FaceColorNumber;
                    CubeletFaces[46].FaceColorNumber = CubeletFaces[45].FaceColorNumber;
                    CubeletFaces[45].FaceColorNumber = CubeletFaces[11].FaceColorNumber;
                    CubeletFaces[11].FaceColorNumber = CubeletFaces[10].FaceColorNumber;
                    CubeletFaces[10].FaceColorNumber = CubeletFaces[9].FaceColorNumber;
                    CubeletFaces[9].FaceColorNumber = CubeletFaces[38].FaceColorNumber;
                    CubeletFaces[38].FaceColorNumber = CubeletFaces[37].FaceColorNumber;
                    CubeletFaces[37].FaceColorNumber = CubeletFaces[36].FaceColorNumber;
                    CubeletFaces[36].FaceColorNumber = CubeletFaces[33].FaceColorNumber;
                    CubeletFaces[33].FaceColorNumber = CubeletFaces[34].FaceColorNumber;
                    CubeletFaces[34].FaceColorNumber = CubeletFaces[35].FaceColorNumber;
                    CubeletFaces[35].FaceColorNumber = temp;
                }
            }
            else
            {
                for (int I = 0; I <= 1; I++)
                {
                    temp = CubeletFaces[0].FaceColorNumber;
                    CubeletFaces[0].FaceColorNumber = CubeletFaces[3].FaceColorNumber;
                    CubeletFaces[3].FaceColorNumber = CubeletFaces[6].FaceColorNumber;
                    CubeletFaces[6].FaceColorNumber = CubeletFaces[7].FaceColorNumber;
                    CubeletFaces[7].FaceColorNumber = CubeletFaces[8].FaceColorNumber;
                    CubeletFaces[8].FaceColorNumber = CubeletFaces[5].FaceColorNumber;
                    CubeletFaces[5].FaceColorNumber = CubeletFaces[2].FaceColorNumber;
                    CubeletFaces[2].FaceColorNumber = CubeletFaces[1].FaceColorNumber;
                    CubeletFaces[1].FaceColorNumber = temp;
                }
                for (int I = 0; I <= 2; I++)
                {
                    temp = CubeletFaces[35].FaceColorNumber;
                    CubeletFaces[35].FaceColorNumber = CubeletFaces[34].FaceColorNumber;
                    CubeletFaces[34].FaceColorNumber = CubeletFaces[33].FaceColorNumber;
                    CubeletFaces[33].FaceColorNumber = CubeletFaces[36].FaceColorNumber;
                    CubeletFaces[36].FaceColorNumber = CubeletFaces[37].FaceColorNumber;
                    CubeletFaces[37].FaceColorNumber = CubeletFaces[38].FaceColorNumber;
                    CubeletFaces[38].FaceColorNumber = CubeletFaces[9].FaceColorNumber;
                    CubeletFaces[9].FaceColorNumber = CubeletFaces[10].FaceColorNumber;
                    CubeletFaces[10].FaceColorNumber = CubeletFaces[11].FaceColorNumber;
                    CubeletFaces[11].FaceColorNumber = CubeletFaces[45].FaceColorNumber;
                    CubeletFaces[45].FaceColorNumber = CubeletFaces[46].FaceColorNumber;
                    CubeletFaces[46].FaceColorNumber = CubeletFaces[47].FaceColorNumber;
                    CubeletFaces[47].FaceColorNumber = temp;
                }
            }
        }

        public void RotateRIGHT2D(bool ClockWise)
        {
            int temp;
            if (ClockWise)
            {
                for (int I = 0; I <= 1; I++)
                {
                    temp = CubeletFaces[18].FaceColorNumber;
                    CubeletFaces[18].FaceColorNumber = CubeletFaces[19].FaceColorNumber;
                    CubeletFaces[19].FaceColorNumber = CubeletFaces[20].FaceColorNumber;
                    CubeletFaces[20].FaceColorNumber = CubeletFaces[23].FaceColorNumber;
                    CubeletFaces[23].FaceColorNumber = CubeletFaces[26].FaceColorNumber;
                    CubeletFaces[26].FaceColorNumber = CubeletFaces[25].FaceColorNumber;
                    CubeletFaces[25].FaceColorNumber = CubeletFaces[24].FaceColorNumber;
                    CubeletFaces[24].FaceColorNumber = CubeletFaces[21].FaceColorNumber;
                    CubeletFaces[21].FaceColorNumber = temp;
                }
                for (int I = 0; I <= 2; I++)
                {
                    temp = CubeletFaces[29].FaceColorNumber;
                    CubeletFaces[29].FaceColorNumber = CubeletFaces[28].FaceColorNumber;
                    CubeletFaces[28].FaceColorNumber = CubeletFaces[27].FaceColorNumber;
                    CubeletFaces[27].FaceColorNumber = CubeletFaces[42].FaceColorNumber;
                    CubeletFaces[42].FaceColorNumber = CubeletFaces[43].FaceColorNumber;
                    CubeletFaces[43].FaceColorNumber = CubeletFaces[44].FaceColorNumber;
                    CubeletFaces[44].FaceColorNumber = CubeletFaces[15].FaceColorNumber;
                    CubeletFaces[15].FaceColorNumber = CubeletFaces[16].FaceColorNumber;
                    CubeletFaces[16].FaceColorNumber = CubeletFaces[17].FaceColorNumber;
                    CubeletFaces[17].FaceColorNumber = CubeletFaces[51].FaceColorNumber;
                    CubeletFaces[51].FaceColorNumber = CubeletFaces[52].FaceColorNumber;
                    CubeletFaces[52].FaceColorNumber = CubeletFaces[53].FaceColorNumber;
                    CubeletFaces[53].FaceColorNumber = temp;
                }
            }
            else
            {
                for (int I = 0; I <= 1; I++)
                {
                    temp = CubeletFaces[18].FaceColorNumber;
                    CubeletFaces[18].FaceColorNumber = CubeletFaces[21].FaceColorNumber;
                    CubeletFaces[21].FaceColorNumber = CubeletFaces[24].FaceColorNumber;
                    CubeletFaces[24].FaceColorNumber = CubeletFaces[25].FaceColorNumber;
                    CubeletFaces[25].FaceColorNumber = CubeletFaces[26].FaceColorNumber;
                    CubeletFaces[26].FaceColorNumber = CubeletFaces[23].FaceColorNumber;
                    CubeletFaces[23].FaceColorNumber = CubeletFaces[20].FaceColorNumber;
                    CubeletFaces[20].FaceColorNumber = CubeletFaces[19].FaceColorNumber;
                    CubeletFaces[19].FaceColorNumber = temp;
                }
                for (int I = 0; I <= 2; I++)
                {
                    temp = CubeletFaces[27].FaceColorNumber;
                    CubeletFaces[27].FaceColorNumber = CubeletFaces[28].FaceColorNumber;
                    CubeletFaces[28].FaceColorNumber = CubeletFaces[29].FaceColorNumber;
                    CubeletFaces[29].FaceColorNumber = CubeletFaces[53].FaceColorNumber;
                    CubeletFaces[53].FaceColorNumber = CubeletFaces[52].FaceColorNumber;
                    CubeletFaces[52].FaceColorNumber = CubeletFaces[51].FaceColorNumber;
                    CubeletFaces[51].FaceColorNumber = CubeletFaces[17].FaceColorNumber;
                    CubeletFaces[17].FaceColorNumber = CubeletFaces[16].FaceColorNumber;
                    CubeletFaces[16].FaceColorNumber = CubeletFaces[15].FaceColorNumber;
                    CubeletFaces[15].FaceColorNumber = CubeletFaces[44].FaceColorNumber;
                    CubeletFaces[44].FaceColorNumber = CubeletFaces[43].FaceColorNumber;
                    CubeletFaces[43].FaceColorNumber = CubeletFaces[42].FaceColorNumber;
                    CubeletFaces[42].FaceColorNumber = temp;
                }
            }
        }

        public void RotateFRONT2D(bool ClockWise)
        {
            int temp;
            if (ClockWise)
            {
                for (int I = 0; I <= 1; I++)
                {
                    temp = CubeletFaces[9].FaceColorNumber;
                    CubeletFaces[9].FaceColorNumber = CubeletFaces[10].FaceColorNumber;
                    CubeletFaces[10].FaceColorNumber = CubeletFaces[11].FaceColorNumber;
                    CubeletFaces[11].FaceColorNumber = CubeletFaces[14].FaceColorNumber;
                    CubeletFaces[14].FaceColorNumber = CubeletFaces[17].FaceColorNumber;
                    CubeletFaces[17].FaceColorNumber = CubeletFaces[16].FaceColorNumber;
                    CubeletFaces[16].FaceColorNumber = CubeletFaces[15].FaceColorNumber;
                    CubeletFaces[15].FaceColorNumber = CubeletFaces[12].FaceColorNumber;
                    CubeletFaces[12].FaceColorNumber = temp;
                }
                for (int I = 0; I <= 2; I++)
                {
                    temp = CubeletFaces[38].FaceColorNumber;
                    CubeletFaces[38].FaceColorNumber = CubeletFaces[6].FaceColorNumber;
                    CubeletFaces[6].FaceColorNumber = CubeletFaces[7].FaceColorNumber;
                    CubeletFaces[7].FaceColorNumber = CubeletFaces[8].FaceColorNumber;
                    CubeletFaces[8].FaceColorNumber = CubeletFaces[45].FaceColorNumber;
                    CubeletFaces[45].FaceColorNumber = CubeletFaces[48].FaceColorNumber;
                    CubeletFaces[48].FaceColorNumber = CubeletFaces[51].FaceColorNumber;
                    CubeletFaces[51].FaceColorNumber = CubeletFaces[20].FaceColorNumber;
                    CubeletFaces[20].FaceColorNumber = CubeletFaces[19].FaceColorNumber;
                    CubeletFaces[19].FaceColorNumber = CubeletFaces[18].FaceColorNumber;
                    CubeletFaces[18].FaceColorNumber = CubeletFaces[44].FaceColorNumber;
                    CubeletFaces[44].FaceColorNumber = CubeletFaces[41].FaceColorNumber;
                    CubeletFaces[41].FaceColorNumber = temp;
                }
            }
            else
            {
                for (int I = 0; I <= 1; I++)
                {
                    temp = CubeletFaces[9].FaceColorNumber;
                    CubeletFaces[9].FaceColorNumber = CubeletFaces[12].FaceColorNumber;
                    CubeletFaces[12].FaceColorNumber = CubeletFaces[15].FaceColorNumber;
                    CubeletFaces[15].FaceColorNumber = CubeletFaces[16].FaceColorNumber;
                    CubeletFaces[16].FaceColorNumber = CubeletFaces[17].FaceColorNumber;
                    CubeletFaces[17].FaceColorNumber = CubeletFaces[14].FaceColorNumber;
                    CubeletFaces[14].FaceColorNumber = CubeletFaces[11].FaceColorNumber;
                    CubeletFaces[11].FaceColorNumber = CubeletFaces[10].FaceColorNumber;
                    CubeletFaces[10].FaceColorNumber = temp;
                }
                for (int I = 0; I <= 2; I++)
                {
                    temp = CubeletFaces[38].FaceColorNumber;
                    CubeletFaces[38].FaceColorNumber = CubeletFaces[41].FaceColorNumber;
                    CubeletFaces[41].FaceColorNumber = CubeletFaces[44].FaceColorNumber;
                    CubeletFaces[44].FaceColorNumber = CubeletFaces[18].FaceColorNumber;
                    CubeletFaces[18].FaceColorNumber = CubeletFaces[19].FaceColorNumber;
                    CubeletFaces[19].FaceColorNumber = CubeletFaces[20].FaceColorNumber;
                    CubeletFaces[20].FaceColorNumber = CubeletFaces[51].FaceColorNumber;
                    CubeletFaces[51].FaceColorNumber = CubeletFaces[48].FaceColorNumber;
                    CubeletFaces[48].FaceColorNumber = CubeletFaces[45].FaceColorNumber;
                    CubeletFaces[45].FaceColorNumber = CubeletFaces[8].FaceColorNumber;
                    CubeletFaces[8].FaceColorNumber = CubeletFaces[7].FaceColorNumber;
                    CubeletFaces[7].FaceColorNumber = CubeletFaces[6].FaceColorNumber;
                    CubeletFaces[6].FaceColorNumber = temp;
                }
            }
        }

        public void RotateBACK2D(bool ClockWise)
        {
            int temp;
            if (ClockWise)
            {
                for (int I = 0; I <= 1; I++)
                {
                    temp = CubeletFaces[27].FaceColorNumber;
                    CubeletFaces[27].FaceColorNumber = CubeletFaces[28].FaceColorNumber;
                    CubeletFaces[28].FaceColorNumber = CubeletFaces[29].FaceColorNumber;
                    CubeletFaces[29].FaceColorNumber = CubeletFaces[32].FaceColorNumber;
                    CubeletFaces[32].FaceColorNumber = CubeletFaces[35].FaceColorNumber;
                    CubeletFaces[35].FaceColorNumber = CubeletFaces[34].FaceColorNumber;
                    CubeletFaces[34].FaceColorNumber = CubeletFaces[33].FaceColorNumber;
                    CubeletFaces[33].FaceColorNumber = CubeletFaces[30].FaceColorNumber;
                    CubeletFaces[30].FaceColorNumber = temp;
                }
                for (int I = 0; I <= 2; I++)
                {
                    temp = CubeletFaces[42].FaceColorNumber;
                    CubeletFaces[42].FaceColorNumber = CubeletFaces[24].FaceColorNumber;
                    CubeletFaces[24].FaceColorNumber = CubeletFaces[25].FaceColorNumber;
                    CubeletFaces[25].FaceColorNumber = CubeletFaces[26].FaceColorNumber;
                    CubeletFaces[26].FaceColorNumber = CubeletFaces[53].FaceColorNumber;
                    CubeletFaces[53].FaceColorNumber = CubeletFaces[50].FaceColorNumber;
                    CubeletFaces[50].FaceColorNumber = CubeletFaces[47].FaceColorNumber;
                    CubeletFaces[47].FaceColorNumber = CubeletFaces[2].FaceColorNumber;
                    CubeletFaces[2].FaceColorNumber = CubeletFaces[1].FaceColorNumber;
                    CubeletFaces[1].FaceColorNumber = CubeletFaces[0].FaceColorNumber;
                    CubeletFaces[0].FaceColorNumber = CubeletFaces[36].FaceColorNumber;
                    CubeletFaces[36].FaceColorNumber = CubeletFaces[39].FaceColorNumber;
                    CubeletFaces[39].FaceColorNumber = temp;
                }
            }
            else
            {
                for (int I = 0; I <= 1; I++)
                {
                    temp = CubeletFaces[27].FaceColorNumber;
                    CubeletFaces[27].FaceColorNumber = CubeletFaces[30].FaceColorNumber;
                    CubeletFaces[30].FaceColorNumber = CubeletFaces[33].FaceColorNumber;
                    CubeletFaces[33].FaceColorNumber = CubeletFaces[34].FaceColorNumber;
                    CubeletFaces[34].FaceColorNumber = CubeletFaces[35].FaceColorNumber;
                    CubeletFaces[35].FaceColorNumber = CubeletFaces[32].FaceColorNumber;
                    CubeletFaces[32].FaceColorNumber = CubeletFaces[29].FaceColorNumber;
                    CubeletFaces[29].FaceColorNumber = CubeletFaces[28].FaceColorNumber;
                    CubeletFaces[28].FaceColorNumber = temp;
                }
                for (int I = 0; I <= 2; I++)
                {
                    temp = CubeletFaces[42].FaceColorNumber;
                    CubeletFaces[42].FaceColorNumber = CubeletFaces[39].FaceColorNumber;
                    CubeletFaces[39].FaceColorNumber = CubeletFaces[36].FaceColorNumber;
                    CubeletFaces[36].FaceColorNumber = CubeletFaces[0].FaceColorNumber;
                    CubeletFaces[0].FaceColorNumber = CubeletFaces[1].FaceColorNumber;
                    CubeletFaces[1].FaceColorNumber = CubeletFaces[2].FaceColorNumber;
                    CubeletFaces[2].FaceColorNumber = CubeletFaces[47].FaceColorNumber;
                    CubeletFaces[47].FaceColorNumber = CubeletFaces[50].FaceColorNumber;
                    CubeletFaces[50].FaceColorNumber = CubeletFaces[53].FaceColorNumber;
                    CubeletFaces[53].FaceColorNumber = CubeletFaces[26].FaceColorNumber;
                    CubeletFaces[26].FaceColorNumber = CubeletFaces[25].FaceColorNumber;
                    CubeletFaces[25].FaceColorNumber = CubeletFaces[24].FaceColorNumber;
                    CubeletFaces[24].FaceColorNumber = temp;
                }
            }
        }

        #endregion

        public void ToggleColor(Point Pt)
        {
            for (int I = 0; I < CubeletFaces.Count(); I++)
            {
                if (CubeletFaces[I].Contains(Pt))
                {
                    CubeletFaces[I].ToggleColor();
                }
            }
        }
    }
}
