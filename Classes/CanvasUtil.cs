using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MusicViz.Classes
{
    public class CanvasUtil
    {
        public static CanvasUtil Instance { get; internal set; } = new CanvasUtil();

        private Canvas _canvas { get; set; }

        public double Width { get => _canvas.ActualWidth; }
        public double Height { get => _canvas.ActualHeight; }

        internal static void SetCanvas(Canvas mainCanvas)
        {
            Instance._canvas = mainCanvas;
        }
    }
}
