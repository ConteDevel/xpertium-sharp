namespace XpertiumSharp.Anfis
{
    public class Anfis
    {
        private readonly XInput input;
        private readonly double[] memory;
        private readonly double[,] output;

        public Anfis(int paramsCount, int phasesCount)
        {
            input = new XInput(paramsCount, phasesCount);
            memory = new double[phasesCount];
            output = new double[phasesCount, paramsCount + 1];
        }

        private void ExecLayer1(double[] data)
        {
            for (int i = 0; i < input.PhasesCount; ++i)
            {
                memory[i] = 1.0f;

                for (int p = 0; p < input.ParamsCount; ++p)
                {
                    memory[i] *= input.GetOutput(p, i, data[p]);
                }
            }
        }

        private void ExecLayer2()
        {
            double sum = 0.0;

            for (int i = 0; i < memory.Length; ++i)
            {
                sum += memory[i];
            }

            for (int i = 0; i < memory.Length; ++i)
            {
                memory[i] /= sum;
            }
        }

        private void ExecLayer3(double[] data)
        {
            for (int i = 0; i < memory.Length; ++i)
            {
                double value = 0.0;

                for (int j = 0; j < input.ParamsCount; ++j)
                {
                    value += output[i, j] * data[j];
                }

                value += output[i, input.ParamsCount];
                memory[i] *= value;
            }
        }
    }
}
