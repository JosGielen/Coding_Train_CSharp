using System.Windows;

namespace Space_Colonization_Tree
{
    internal class Leaf
    {
        private Vector my_Location;
        private bool my_Alive;

        public Leaf(Vector loc)
        {
            my_Location = loc;
            my_Alive = true;
        }

        public Vector Location
        {
            get { return my_Location; }
            set { my_Location = value; }
        }

        public bool Alive
        {
            get { return my_Alive; }
        }

        public Branch ClosestBranch(List<Branch> branches, double killDist, double viewDist)
        {
            double dist = 0.0;
            double mindist = double.MaxValue;
            Branch closest = null;
            for (int I = 0; I < branches.Count; I++)
            {
                dist = Math.Sqrt((my_Location.X - branches[I].Location.X) * (my_Location.X - branches[I].Location.X) + (my_Location.Y - branches[I].Location.Y) * (my_Location.Y - branches[I].Location.Y));
                if (dist < killDist)
                {
                    my_Alive = false;
                    return null;
                }
                if (dist < viewDist)
                {
                    if (dist < mindist)
                    {
                        mindist = dist;
                        closest = branches[I];
                    }
                }
            }
            return closest;
        }
    }
}
