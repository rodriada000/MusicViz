using MusicViz.Classes;
using System;
using System.Windows.Shapes;

namespace MusicViz.Shapes
{
    public class VizShape
    {
        public double x;
        public double y;
        public double min_size;
        public double max_size;
        public Shape? element;
        public FreqRange ReactRange { get; set; }

        public VizShape(double x, double y, Shape element)
        {
            this.x = x;
            this.y = y;
            this.element = element;
        }

        public virtual void Update(FFTResult results, float[] rawBuffer)
        {

        }
    }


}
