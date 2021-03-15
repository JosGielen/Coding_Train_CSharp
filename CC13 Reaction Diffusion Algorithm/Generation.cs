using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiffReaction
{
    class Generation
    {
        private double[,] CellsA;
        private double[,] CellsB;
        private double[,] NextA;
        private double[,] NextB;
        private long m_Volgnummer;
        private int m_Breedte;
        private int m_Hoogte;
        private double m_DiffA;
        private double m_DiffB;
        private double m_Feed;
        private double m_Kill;

        public Generation(int Breedte, int Hoogte)
        {
            CellsA = new double[Breedte, Hoogte];
            CellsB = new double[Breedte, Hoogte];
            NextA = new double[Breedte, Hoogte];
            NextB = new double[Breedte, Hoogte];
            for (int X = 0; X < Breedte; X++)
            {
                for (int Y = 0; Y < Hoogte; Y++)
                {
                    CellsA[X, Y] = 1.0;
                    CellsB[X, Y] = 0.0;
                }
            }
            m_Volgnummer = 1;
            m_Breedte = Breedte;
            m_Hoogte = Hoogte;
            //Appy a seed area
            for (int X = Breedte / 2 - 5; X <= Breedte / 2 + 5; X++)
            {
                for (int Y = Hoogte / 2 - 5; Y <= Hoogte / 2 + 5; Y++)
                {
                    CellsB[X, Y] = 1.0;
                }
            }
        }

        public long Volgnummer
        {
            get { return m_Volgnummer; }
        }

        public int Breedte
        {
            get { return m_Breedte; }

        }

        public int Hoogte
        {
            get { return m_Hoogte; }
        }

        public double GetCellA(int X, int Y)
        {
            if (X >= 0 & X < m_Breedte & Y >= 0 & Y < m_Hoogte)
            {
                return CellsA[X, Y];
            }
            else
            {
                return 1.0;

            }
        }

        public void SetCellA(int X, int Y, double value)
        {
            if (X >= 0 & X < m_Breedte & Y >= 0 & Y < m_Hoogte)
            {
                CellsA[X, Y] = value;
            }
            else
            {
                CellsA[X, Y] = 1.0;
            }

        }

        public double GetCellB(int X, int Y)
        {
            if (X >= 0 & X < m_Breedte & Y >= 0 & Y < m_Hoogte)
            {
                return CellsB[X, Y];
            }
            else
            {
                return 0.0;

            }
        }

        public void SetCellB(int X, int Y, double value)
        {
            if (X >= 0 & X < m_Breedte & Y >= 0 & Y < m_Hoogte)
            {
                CellsB[X, Y] = value;
            }
            else
            {
                CellsB[X, Y] = 0.0;
            }

        }

        public double DiffA
        {
            get { return m_DiffA; }
            set { m_DiffA = value; }
        }

        public double DiffB
        {
            get { return m_DiffB; }
            set { m_DiffB = value; }
        }

        public double Feed
        {
            get { return m_Feed; }
            set { m_Feed = value; }
        }

        public double Kill
        {
            get { return m_Kill; }
            set { m_Kill = value; }
        }

        public void update()
        {
            //Use the Diffusion-Reaction algorithm to create a new generation in }A and }B
            double LaplaceA;
            double LaplaceB;
            for (int X = 0; X < m_Breedte; X++)
            {
                for (int Y = 0; Y < m_Hoogte; Y++)
                {
                    //Calculate the Laplacian 3x3 convolution for each cell A and B
                    LaplaceA = -1.0 * CellsA[X, Y];
                    LaplaceB = -1.0 * CellsB[X, Y];
                    if (X > 0 & Y > 0)
                    {
                        LaplaceA += 0.05 * CellsA[X - 1, Y - 1];
                        LaplaceB += 0.05 * CellsB[X - 1, Y - 1];
                    }
                    if (X > 0)
                    {
                        LaplaceA += 0.2 * CellsA[X - 1, Y];
                        LaplaceB += 0.2 * CellsB[X - 1, Y];
                    }
                    if (X > 0 & Y < m_Hoogte - 1)
                    {
                        LaplaceA += 0.05 * CellsA[X - 1, Y + 1];
                        LaplaceB += 0.05 * CellsB[X - 1, Y + 1];
                    }
                    if (Y > 0)
                    {
                        LaplaceA += 0.2 * CellsA[X, Y - 1];
                        LaplaceB += 0.2 * CellsB[X, Y - 1];
                    }
                    if (Y < m_Hoogte - 1)
                    {
                        LaplaceA += 0.2 * CellsA[X, Y + 1];
                        LaplaceB += 0.2 * CellsB[X, Y + 1];
                    }
                    if (X < m_Breedte - 1 & Y > 0)
                    {
                        LaplaceA += 0.05 * CellsA[X + 1, Y - 1];
                        LaplaceB += 0.05 * CellsB[X + 1, Y - 1];
                    }
                    if (X < m_Breedte - 1)
                    {
                        LaplaceA += 0.2 * CellsA[X + 1, Y];
                        LaplaceB += 0.2 * CellsB[X + 1, Y];
                    }
                    if (X < m_Breedte - 1 & Y < m_Hoogte - 1)
                    {
                        LaplaceA += 0.05 * CellsA[X + 1, Y + 1];
                        LaplaceB += 0.05 * CellsB[X + 1, Y + 1];
                    }
                    //Calculate NextA and NextB
                    NextA[X, Y] = CellsA[X, Y] + m_DiffA * LaplaceA - CellsA[X, Y] * CellsB[X, Y] * CellsB[X, Y] + Feed * (1 - CellsA[X, Y]);
                    NextB[X, Y] = CellsB[X, Y] + m_DiffB * LaplaceB + CellsA[X, Y] * CellsB[X, Y] * CellsB[X, Y] - (Kill + Feed) * CellsB[X, Y];
                }
            }
            //Copy Next back to Cells
            for (int X = 0; X < m_Breedte; X++)
            {
                for (int Y = 0; Y < m_Hoogte; Y++)
                {
                    CellsA[X, Y] = NextA[X, Y];
                    CellsB[X, Y] = NextB[X, Y];
                }
            }
            m_Volgnummer += 1;
        }

        public void Clear()
        {
            for (int X = 0; X < m_Breedte; X++)
            {
                for (int Y = 0; Y < m_Hoogte; Y++)
                {
                    CellsA[X, Y] = 1.0;
                    CellsB[X, Y] = 0.0;
                }
            }
        }
    }
}
