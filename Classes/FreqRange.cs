using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicViz.Classes
{
    public class FreqRange
    {
        public FreqRange(double min, double max)
        {
            Min = min;
            Max = max;
        }

        public double Min { get; set; }
        public double Max { get; set; }

        public bool Between(double val)
        {
            return val >= Min && val < Max;
        }
    }
}
