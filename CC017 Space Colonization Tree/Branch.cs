using System.Windows;

namespace Space_Colonization_Tree
{
    internal class Branch
    {
        private Branch my_Parent;
        private Vector my_Location;
        private Vector my_Direction;
        private Vector my_ForceDir;
        private int my_ForceCount;
        private double my_Length;

        public Branch(Branch parent, Vector loc, Vector dir, double length)
        {
            my_Parent = parent;
            my_Location = loc;
            my_ForceDir = dir;
            my_Direction = dir;
            my_Length = length;
            my_ForceCount = 0;
            my_Direction.Normalize();
            my_Direction = length * my_Direction;
        }

        public Branch Parent
        {
            get { return my_Parent; }
        }

        public Vector Location
        {
            get { return my_Location; }
        }

        public int ForceCount
        {
            get { return my_ForceCount; }
        }

        public void Reset()
        {
            my_ForceCount = 0;
            my_ForceDir = my_Direction;
        }

        public void AddForce(Vector force)
        {
            my_ForceDir = my_ForceDir + force;
            my_ForceCount += 1;
        }

        public Branch Spawn()
        {
            Branch result = null;
            if (my_ForceCount > 0)
            {
                my_ForceDir = my_ForceDir / my_ForceCount;
                my_ForceDir.Normalize();
                my_ForceDir = my_Length * my_ForceDir;
                result = new Branch(this, my_Location + my_ForceDir, my_ForceDir, my_Length);
            }
            else
            {
                result = new Branch(this, my_Location + my_Direction, my_Direction, my_Length);
            }
            return result;
        }
    }
}
