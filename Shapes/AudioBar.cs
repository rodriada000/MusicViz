using MusicViz.Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace MusicViz.Shapes
{
    public class AudioBar : VizShape
    {
        public double scale = 5;

        public AudioBar(double x, double y, Shape element, FreqRange range, bool isFlipped = true) : base(x, y, element)
        {
            element.Margin = new System.Windows.Thickness(x, y, 0, 0);
            this.ReactRange = range;
            this.max_size = 128;

            this.element.RenderTransform = isFlipped ? new System.Windows.Media.ScaleTransform(1, -1) : null;
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


            if (relevantPower.Count == 0 || Math.Abs(relevantPower.Max()) == double.PositiveInfinity)
            {
                element.Height = 0;
            }
            else
            {
                element.Height = Math.Min(Math.Abs(relevantPower.Max()) * (CanvasUtil.Instance.Height * 0.75) * scale, CanvasUtil.Instance.Height * 0.75);
            }

        }
    }


}
