using System;

namespace XpertiumSharp.Utils
{
    public sealed class XMath
    {
        private XMath() { }

        public static double Gaussian(double x, double c, double s)
        {
            double y = (x - c) / s;
            return Math.Exp(-y * y);
        }
    }
}
