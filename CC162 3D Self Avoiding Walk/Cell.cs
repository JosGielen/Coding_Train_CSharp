using System.Windows.Media;
using System.Windows.Media.Media3D;
using JG_GL;

namespace Self_Avoiding_Walk
{
    internal class Cell
    {
        private int my_X;
        private int my_Y;
        private int my_Z;
        private int max;
        private bool my_Used;
        private bool[] Tried;
        private bool[] Available;

        public Cell(int x, int y, int z, int maxSize)
        {
            my_X = x; 
            my_Y = y; 
            my_Z = z;
            max = maxSize;
            my_Used = false;
            Tried = [false, false, false, false, false, false]; //Left, Right, Top, Bottom, Front, Back
            Available = [true, true, true, true, true, true];
        }

        public int X 
        {
            get { return my_X; }
            set { my_X = value; }
        }

        public int Y
        { 
            get { return my_Y; } 
            set { my_Y = value; }
        }

        public int Z
        {
            get { return my_Z; }
            set { my_Z = value; }
        }

        public bool Used
        { 
            get { return my_Used; } 
            set { my_Used = value; }
        }

        public List<int> FreeNeighbours(Cell[,,] cells)
        {
            List<int> result = new List<int>();
            Available[0] = true;
            if (my_X > 0)
            {
                if (cells[my_X - 1, my_Y, my_Z].my_Used) { Available[0] = false; } //Left
            }
            else
            {
                Available[0] = false;
            }
            Available[1] = true;
            if (my_X < max - 1)
            {
                if (cells[my_X + 1, my_Y, my_Z].my_Used) { Available[1] = false; } //Right
            }
            else
            { 
                Available[1] = false; 
            }
            Available[2] = true;
            if (my_Y > 0)
            {
                if (cells[my_X, my_Y - 1, my_Z].my_Used) { Available[2] = false; } //Top
            }
            else 
            { 
                Available[2] = false; 
            }
            Available[3] = true;
            if (my_Y < max - 1)
            {
                if (cells[my_X, my_Y + 1, my_Z].my_Used) { Available[3] = false; } //Bottom
            }
            else
            {
                Available[3] = false;
            }
            Available[4] = true;
            if (my_Z > 0)
            {
                if (cells[my_X, my_Y, my_Z - 1].my_Used) { Available[4] = false; } //Front
            }
            else
            {
                Available[4] = false;
            }
            Available[5] = true;
            if (my_Z < max - 1)
            {
                if (cells[my_X, my_Y, my_Z + 1].my_Used) { Available[5] = false; } //Back
            }
            else
            {
                Available[5] = false;
            }
            for (int i = 0; i < 6; i++)
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
            Tried = [false, false, false, false, false, false];
        }

        public void Draw(GLScene scene, int step)
        {
            EllipsoidGeometry El = new EllipsoidGeometry(0.4, 0.4, 0.4, 8, 8)
            {
                Position = new Vector3D((my_X - max / 2) * step, (my_Y - max / 2) * step, (my_Z - max / 2) * step),
                InitialRotationAxis = new Vector3D(0, 0, 0),
                RotationAxis = new Vector3D(0, 1, 0),
                RotationSpeed = 0.0,
                TextureScaleX = 1.0,
                TextureScaleY = 1.0,
                DrawMode = DrawMode.Fill
            };
            El.DiffuseMaterial = Color.FromRgb(200, 200, 200);
            El.AmbientMaterial = Color.FromRgb(100, 100, 100);
            El.SpecularMaterial = Color.FromRgb(100, 100, 100);
            El.Shininess = 5.0;
            El.UseMaterial = true;
            scene.AddGeometry(El);
        }
    }
}
