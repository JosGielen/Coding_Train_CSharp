
namespace WinLife
{
    internal class Generation
    {
        private bool[,] Cells;
        private bool[,] Dummy;
        private long m_Volgnummer;
        private int m_Breedte;
        private int m_Hoogte;

        public Generation()
        {
            Cells = new bool[200, 200];
            Dummy = new bool[200, 200];
            for (int X = 0; X < 200; X++)
            {
                for (int Y = 0; Y < 200; Y++)
                {
                    Cells[X, Y] = false;
                }
            }
            m_Volgnummer = 1;
            m_Breedte = 200;
            m_Hoogte = 200;
        }

        public Generation(int Breedte, int Hoogte)
        {
            Cells = new bool[Breedte, Hoogte];
            Dummy = new bool[Breedte, Hoogte];
            for (int X = 0; X < Breedte; X++)
            {
                for (int Y = 0; Y < Hoogte; Y++)
                {
                    Cells[X, Y] = false;
                }
            }
            m_Volgnummer = 1;
            m_Breedte = Breedte;
            m_Hoogte = Hoogte;
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

        public bool GetCell(int X, int Y)
        {
            if (X >= 0 & X < m_Breedte & Y >= 0 & Y < m_Hoogte)
            {
                return Cells[X, Y];
            }
            else
            {
                return false;
            }
        }

        public void SetCell(int X, int Y, bool value)
        {
            if (X >= 0 & X < m_Breedte & Y >= 0 & Y < m_Hoogte)
            {
                Cells[X, Y] = value;
            }
            else
            {
                Cells[X, Y] = false;
            }
        }

        public void update()
        {
            //Use Game of Life rules to create a new generation in Dummy
            int buren;
            for (int X = 0; X < m_Breedte; X++)
            {
                for (int Y = 0; Y < m_Hoogte; Y++)
                {
                    buren = 0;
                    if (X > 0 & Y > 0)
                    {
                        if (Cells[X - 1, Y - 1]) buren++;
                    }
                    if (X > 0)
                    {
                        if (Cells[X - 1, Y]) buren++;
                    }
                    if (X > 0 & Y < m_Hoogte - 1)
                    {
                        if (Cells[X - 1, Y + 1]) buren++;
                    }
                    if (Y > 0)
                    {
                        if (Cells[X, Y - 1]) buren++;
                    }
                    if (Y < m_Hoogte - 1)
                    {
                        if (Cells[X, Y + 1]) buren++;
                    }
                    if (X < m_Breedte - 1 & Y > 0)
                    {
                        if (Cells[X + 1, Y - 1]) buren++;
                    }
                    if (X < m_Breedte - 1)
                    {
                        if (Cells[X + 1, Y]) buren++;
                    }
                    if (X < m_Breedte - 1 & Y < m_Hoogte - 1)
                    {
                        if (Cells[X + 1, Y + 1]) buren++;
                    }
                    Dummy[X, Y] = Cells[X, Y];
                    if (Dummy[X, Y] == true & buren < 2) Dummy[X, Y] = false;
                    if (Dummy[X, Y] == true & buren > 3) Dummy[X, Y] = false;
                    if (Dummy[X, Y] == false & buren == 3) Dummy[X, Y] = true;
                }
            }
            //Copy Dummy back to Cells
            for (int X = 0; X < m_Breedte; X++)
            {
                for (int Y = 0; Y < m_Hoogte; Y++)
                {
                    Cells[X, Y] = Dummy[X, Y];
                }
            }
            m_Volgnummer++;
        }

        public void Clear()
        {
            for (int X = 0; X < m_Breedte; X++)
            {
                for (int Y = 0; Y < m_Hoogte; Y++)
                {
                    Cells[X, Y] = false;
                }
            }
        }
    }
}
