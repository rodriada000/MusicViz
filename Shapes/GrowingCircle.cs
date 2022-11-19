using MusicViz.Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Shapes;

namespace MusicViz.Shapes
{
    public class GrowingCircle : MovingCircle
    {
        public double last_growth = 0;
        public double scale = 0.25;

        public GrowingCircle(double x, double y, Shape element, FreqRange range) : base(x, y, element)
        {
            var r = new Random();
            scale = r.Next(1, 5);
            this.ReactRange = range;
        }

        public override void Update(FFTResult results, float[] rawBuffer)
        {
            base.Update(results, rawBuffer);

            if (element == null || results == null)
            {
                return;
            }

            List<double> relevantPower = new List<double>();
            int index = 0;

            for (int i = 0; i < results.FFTFreq.Length; i++)
            {
                if (ReactRange.Between(results.FFTFreq[i]))
                {
                    relevantPower.Add(results.FFTMagnitude[i]);
                    index = i;
                }
            }


            if (relevantPower.Count == 0 || Math.Abs(Math.Log(Math.Sqrt(relevantPower.Max()))) == double.PositiveInfinity)
            {
                element.Height = min_size;
                element.Width = element.Height;
            }
            else
            {
                element.Width = Math.Abs(Math.Log(Math.Sqrt(relevantPower.Max() * 100))) * scale;

                if (ReactRange.Between(results.PeakFreq))
                {
                    element.Width *= .5;
                }

                element.Height = element.Width;
            }
        }

        double Lerp(double firstFloat, double secondFloat, double by)
        {
            return firstFloat * (1 - by) + secondFloat * by;
        }
    }


}
