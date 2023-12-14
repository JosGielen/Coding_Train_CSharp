public class Gene
{
    private double my_Dir;
    private double my_Size;
    static Random rnd = new Random();

    public Gene()
    {
        my_Dir = 2 * Math.PI * rnd.NextDouble();
        my_Size = 4 * rnd.NextDouble();
    }

    public double Dir
    {
        get { return my_Dir; }
        set { my_Dir = value; }
    }

    public double Size
    {
        get { return my_Size; }
        set { my_Size = value; }
    }
}
