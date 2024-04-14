using System.Windows;

namespace Space_Colonization_Tree
{
    internal class Tree
    {
        private List<Leaf> my_Leafs;
        private List<Branch> my_Branches;
        private double my_Killdist;
        private double my_Viewdist;
        private bool my_RootGrow;

        public Tree(double killdist, double viewdist)
        {
            my_Killdist = killdist;
            my_Viewdist = viewdist;
            my_RootGrow = true;
            my_Leafs = new List<Leaf>();
            my_Branches = new List<Branch>();
        }

        public List<Leaf> Leafs
        {
            get { return my_Leafs; }
        }

        public List<Branch> Branches
        {
            get { return my_Branches; }
        }

        public void SetRoot(Vector location, Vector Startdirection, double branchlength)
        {
            Branch b = new Branch(null, location, Startdirection, branchlength);
            my_Branches.Add(b);
        }

        public void AddLeaf(Leaf l)
        {
            my_Leafs.Add(l);
        }

        public List<Branch> Grow()
        {
            List<Branch> result = new List<Branch>();
            Branch closestBranch;
            Branch newBranch;
            Vector V;
            if (my_RootGrow == true)
            {
                //Step1: Grow the root of the tree untill leaves are reached
                newBranch = my_Branches.Last();
                double d = 0.0;
                for (int I = 0; I < my_Leafs.Count; I++)
                {
                    d = Math.Sqrt((newBranch.Location.X - my_Leafs[I].Location.X) * (newBranch.Location.X - my_Leafs[I].Location.X) + (newBranch.Location.Y - my_Leafs[I].Location.Y) * (newBranch.Location.Y - my_Leafs[I].Location.Y));
                    if (d < my_Viewdist)
                    {
                        my_RootGrow = false;
                        break;
                    }
                }
                if (my_RootGrow)
                {
                    my_Branches.Add(newBranch.Spawn());
                    result.Add(my_Branches.Last());
                }
            }
            else
            {
                //Step2: Make the branches of the tree
                for (int I = 0; I < my_Leafs.Count; I++)
                {
                    closestBranch = my_Leafs[I].ClosestBranch(my_Branches, my_Killdist, my_Viewdist);
                    if (closestBranch != null)
                    {
                        V = my_Leafs[I].Location - closestBranch.Location;
                        closestBranch.AddForce(V);
                    }
                }
                //Add branches
                for (int I = my_Branches.Count - 1; I >= 0; I--)
                {
                    if (my_Branches[I].ForceCount > 0)
                    {
                        newBranch = my_Branches[I].Spawn();
                        my_Branches.Add(newBranch);
                        result.Add(newBranch);
                        my_Branches[I].Reset();
                    }
                }
                //Remove dead leaves
                for (int I = my_Leafs.Count - 1; I >= 0; I--)
                {
                    if (!my_Leafs[I].Alive) my_Leafs.RemoveAt(I);
                }
            }
            return result;
        }
    }
}
