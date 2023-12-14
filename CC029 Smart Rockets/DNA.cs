public class DNA
{
    private List<Gene> my_Genes;

    public DNA(int lifespan, Random rnd)
    {
        Gene g;
        my_Genes = new List<Gene>();
        for (int I = 0; I < lifespan; I++)
        {
            g = new Gene();
            my_Genes.Add(g);
        }
    }

    public DNA(DNA mate1, DNA mate2, bool randomchoice, bool allowMutation, int lifespan, Random rnd)
    {
        my_Genes = new List<Gene>();
        if (randomchoice)
        {
            //Method1 : Random choice
            double dummy;
            for (int I = 0; I < lifespan; I++)
            {
                dummy = rnd.NextDouble();
                if (dummy < 0.5)
                {
                    my_Genes.Add(mate1.Genes[I]);
                }
                else
                {
                    my_Genes.Add(mate2.Genes[I]);
                }
            }
        }
        else
        {
            //Method2 : Use a midpoint to devide the genes
            int midPt = rnd.Next(lifespan);
            for (int I = 0; I < lifespan; I++)
            {
                if (I < midPt)
                {
                    my_Genes.Add(mate1.Genes[I]);
                }
                else
                {
                    my_Genes.Add(mate2.Genes[I]);
                }
            }
        }
        //Mutation
        if (allowMutation)
        {
            for (int I = 0; I < lifespan; I++)
            {
                if (rnd.NextDouble() < 0.01)
                {
                    my_Genes[I] = new Gene();
                }
            }
        }
    }

    public List<Gene> Genes
    {
        get { return my_Genes; }
    }
}
