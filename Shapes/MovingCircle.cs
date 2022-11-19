using MusicViz.Classes;
using System;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace MusicViz.Shapes
{
    public class MovingCircle : VizShape
    {
        public double velocity = 0.5;
        public double xDirection = 1;
        public double yDirection = 1;

        public MovingCircle(double x, double y, Shape element) : base(x, y, element)
        {
            var r = new Random();

            do
            {
                xDirection = r.Next(-1, 2);
                yDirection = r.Next(-1, 2);
            } while (xDirection == 0 && yDirection == 0);

            velocity = r.NextDouble() + 0.1;

            element.SetValue(Canvas.LeftProperty, x);
            element.SetValue(Canvas.TopProperty, y);
        }

        public override void Update(FFTResult results, float[] rawBuffer)
        {
            base.Update(results, rawBuffer);

            if (element == null)
            {
                return;
            }

            x += xDirection * velocity;
            y += yDirection * velocity;

            if (x < -element.Width)
            {
                x = CanvasUtil.Instance.Width + element.Width;
            }
            else if (x > CanvasUtil.Instance.Width + element.Width)
            {
                x = -element.Width;
            }
            
            if (y < -element.Height)
            {
                y = CanvasUtil.Instance.Height + element.Height;
            }
            else if (y > CanvasUtil.Instance.Height + element.Height)
            {
                y = -element.Height;
            }

            element.SetValue(Canvas.LeftProperty, x);
            element.SetValue(Canvas.TopProperty, y);
        }
    }


}
