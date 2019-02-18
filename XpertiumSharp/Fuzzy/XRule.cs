namespace XpertiumSharp.Fuzzy
{
    public class XRule
    {
        private readonly XPhase[] inputs;
        public readonly double[] Weights;

        public XRule(XPhase[] inputs)
        {
            this.inputs = inputs;
            Weights = new double[inputs.Length + 1];
        }

        public double GetOutput(double[] data)
        {
            double output = 0.0;

            for (int i = 0; i < data.Length; ++i)
            {
                output += Weights[i] * inputs[i].Hit(data[i]);
            }

            output += Weights[data.Length];

            return output;
        }

        public void LoadWeights(double[] weights)
        {
            if (weights.Length == Weights.Length)
            {
                for (int i = 0; i < weights.Length; ++i)
                {
                    Weights[i] = weights[i];
                }
            }
        }
    }
}
