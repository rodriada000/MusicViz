using MusicViz.Classes;
using MusicViz.Extensions;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MusicViz.Shapes
{
    public class WaveFormCircle : VizShape
    {

        /// <summary>
        /// Speed at which waveform rotates
        /// </summary>
        public double AngleSpeed { get; set; } = 0;

        public Polyline Polyline { get => (Polyline) element;  }

        /// <summary>
        /// Current angle of waveform
        /// </summary>
        public double Angle { get; set; }

        PointCollection PolygonPoints = new PointCollection();

        public WaveFormCircle(double x, double y, Shape element) : base(x, y, element)
        {
            min_size = 25;
            max_size = 250;
        }

        public override void Update(FFTResult results, float[] rawBuffer)
        {
            PolygonPoints.Clear();

            if (results == null || results.FFTMagnitude == null || results.FFTMagnitude?.Length == 0)
            {
                return;
            }

            double offset_w = this.x;
            double offset_h = this.y;

            Polyline.RenderTransform = new RotateTransform(Angle, offset_w, offset_h);
            Angle = (Angle + AngleSpeed) % 360;

            for (int i = 0; i <= 180; i++)
            {
                int index = i.Map(0, 180, 0, rawBuffer.Length - 1);

                double radians = i * (Math.PI / 180);
                double r = rawBuffer[index].Map(-1, 1, (float)min_size, (float)max_size);

                double x = r * Math.Sin(radians);
                double y = r * Math.Cos(radians);

                var p = new Point(x + offset_w, y + offset_h);
                PolygonPoints.Add(p);
            }

            for (int i = 180; i >= 0; i--)
            {
                int index = i.Map(0, 180, 0, rawBuffer.Length - 1);

                double radians = i * (Math.PI / 180);
                double r = rawBuffer[index].Map(-1, 1, (float)min_size, (float)max_size);

                double x = r * -Math.Sin(radians);
                double y = r * Math.Cos(radians);

                var p = new Point(x + offset_w, y + offset_h);
                PolygonPoints.Add(p);
            }


            Polyline.Points = PolygonPoints;

            base.Update(results, rawBuffer);
        }

    }
}
