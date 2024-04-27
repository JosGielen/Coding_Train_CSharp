using System.Windows.Controls;

namespace Binary_Tree
{
    internal class BinaryTree
    {
        private double X;
        private double Y;
        private Node Root;

        public BinaryTree(double x, double y)
        {
            X = x;
            Y = y;
            Root = new Node(x, y, 1);
        }

        public bool Add(double key, object item, Canvas canv)
        {
            return Root.Add(key, item, canv);
        }

        public object? GetItem (double key)
        {
            return Root.GetItem(key);
        }

        public void Clear()
        {
            Root = new Node(X, Y, 1);
        }

        public bool Contains(double key)
        {
            return Root.Contains(key);
        }

        public void Modify(double key, object item)
        {
            Root.Modify(key, item);
        }

        public List<double> Keys
        {
            get
            {
                List<double> result = new List<double>();
                Root.AddKeyToList(result);
                return result;
            }
        }
    }
}
