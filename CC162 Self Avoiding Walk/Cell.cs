using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Self_Avoiding_Walk
{
    internal class Cell
    {
        private int my_Col;
        private int my_Row;
        private bool my_Used;
        private bool[] Tried;
        private bool[] Available;

        public Cell(int col, int row)
        {
            my_Col = col;
            my_Row = row;
            my_Used = false;
            Tried = [false, false, false, false]; //Left, Right, Top, Bottom
            Available = [true, true, true, true];
        }

        public int Col
        {
            get { return my_Col; }
            set { my_Col = value; }
        }

        public int Row
        { 
            get { return my_Row; } 
            set { my_Row = value; }
        }

        public bool Used
        { 
            get { return my_Used; } 
            set { my_Used = value; }
        }

        public List<int> FreeNeighbours(Cell[,] cells)
        {
            List<int> result = new List<int>();
            int maxcol = cells.GetLength(0) - 1;
            int maxRow = cells.GetLength(1) - 1;
            Available[0] = true;
            if (my_Col > 0)
            {
                if (cells[my_Col - 1, my_Row].my_Used) { Available[0] = false; } //Left
            }
            else
            {
                Available[0] = false;
            }
            Available[1] = true;
            if (my_Col < maxcol)
            {
                if (cells[my_Col + 1, my_Row].my_Used) { Available[1] = false; } //Right
            }
            else
            { 
                Available[1] = false; 
            }
            Available[2] = true;
            if (my_Row > 0)
            {
                if (cells[my_Col, my_Row - 1].my_Used) { Available[2] = false; } //Top
            }
            else 
            { 
                Available[2] = false; 
            }
            Available[3] = true;
            if (my_Row < maxRow)
            {
                if (cells[my_Col, my_Row + 1].my_Used) { Available[3] = false; } //Bottom
            }
            else
            {
                Available[3] = false;
            }
            for (int i = 0; i < 4; i++)
            {
                if (Available[i] && !Tried[i]) { result.Add(i); }
            }
            return result;
        }

        public void SetTried(int dir)
        {
            Tried[dir] = true;
        }

        public void UnTried()
        {
            Tried = [false, false, false, false];
        }

        public void Draw(Canvas c, int step)
        {
            Ellipse El = new Ellipse()
            {
                Width = 8,
                Height = 8,
                Fill = Brushes.Black
            };
            El.SetValue(Canvas.LeftProperty, step * (my_Col + 0.5) - 4.0);
            El.SetValue(Canvas.TopProperty , step * (my_Row + 0.5) - 4.0);
            c.Children.Add(El);
        }
    }
}
