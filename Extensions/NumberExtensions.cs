using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicViz.Extensions
{
    public static class NumberExtensions
    {
        public static int Map(this int from, int fromMin, int fromMax, int toMin, int toMax)
        {
            return (int)Map((double)from, (double)fromMin, (double)fromMax, (double)toMin, (double)toMax);
        }

        public static double Map(this double from, double fromMin, double fromMax, double toMin, double toMax)
        {
            var fromAbs = from - fromMin;
            var fromMaxAbs = fromMax - fromMin;

            var normal = fromAbs / fromMaxAbs;

            var toMaxAbs = toMax - toMin;
            var toAbs = toMaxAbs * normal;

            var to = toAbs + toMin;

            return to;
        }

        public static float Map(this float from, float fromMin, float fromMax, float toMin, float toMax)
        {
            var fromAbs = from - fromMin;
            var fromMaxAbs = fromMax - fromMin;

            var normal = fromAbs / fromMaxAbs;

            var toMaxAbs = toMax - toMin;
            var toAbs = toMaxAbs * normal;

            var to = toAbs + toMin;

            return to;
        }
    }
}
