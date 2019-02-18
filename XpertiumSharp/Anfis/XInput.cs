namespace XpertiumSharp.Anfis
{
    public class XInput
    {
        private readonly XParameter[] parameters;
        public int ParamsCount { get; private set; }
        public int PhasesCount { get; private set; }

        public XInput(int paramsCount, int phasesCount)
        {
            parameters = new XParameter[paramsCount];
            ParamsCount = paramsCount;
            PhasesCount = phasesCount;

            for (int i = 0; i < paramsCount; ++i)
            {
                parameters[i] = new XParameter(phasesCount);
            }
        }

        public double GetOutput(int parameter, int phase, double x)
        {
            return parameters[parameter].Phases[phase].Hit(x);
        }

        public struct XPhase
        {
            public double Center;
            public double S;

            public XPhase(double center, double s)
            {
                Center = center;
                S = s;
            }

            public double Hit(double x)
            {
                return XMath.Gaussian(x, Center, S);
            }
        }

        private struct XParameter
        {
            public readonly XPhase[] Phases;

            public XParameter(int count)
            {
                Phases = new XPhase[count];

                for (int i = 0; i < Phases.Length; ++i)
                {
                    Phases[i] = new XPhase(0.0, 1.0);
                }
            }
        }
    }
}
