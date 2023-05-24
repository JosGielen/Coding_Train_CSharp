

namespace NeuralNet_Test
{
    //XOR NeuralNet Testing
    internal class TrainingData
    {
        public double[] data = new double[2];
        public double[] targets = new double[1];

        public TrainingData(double d1, double d2, double t)
        {
            data[0] = d1;
            data[1] = d2;
            targets[0] = t;
        }
    }
}
