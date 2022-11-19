using MusicViz.Shapes;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MusicViz.Classes
{
    /// <summary>
    /// Class responsible for adding/updating Canvas with shapes
    /// </summary>
    public class VizManager
    {
        /// <summary>
        /// All shapes currently drawn on <see cref="_canvas"/>
        /// </summary>
        public List<VizShape> Shapes { get; set; } = new List<VizShape>();

        /// <summary>
        /// Most recent results of the FFT performed on <see cref="RawBuffer"/>
        /// </summary>
        public FFTResult Results { get; internal set; }
        
        /// <summary>
        /// Raw audio buffer captured from device
        /// </summary>
        public float[] RawBuffer { get; set; }

        private Canvas _canvas;

        private Random _randGen = new Random();

        public VizManager(Canvas canvas)
        {
            _canvas = canvas;
        }

        public void AddMovingCircle(int x, int y, int width, int height)
        {
            var color = new SolidColorBrush(Color.FromArgb((byte)_randGen.Next(5, 220), (byte)_randGen.Next(0, 255), (byte)_randGen.Next(0, 255), (byte)_randGen.Next(0, 255)));

            Ellipse circle = new Ellipse()
            {
                Width = width,
                Height = height,
                Stroke = color,
                StrokeThickness = 2,
                Fill = color
            };

            Shapes.Add(new MovingCircle(x, y, circle) { min_size = Math.Max(5, width - 10), max_size = width + 10 });

            _canvas.Children.Add(circle);
        }

        public void AddGrowingCircle(int x, int y, int width, int height, FreqRange range)
        {
            var color = new SolidColorBrush(Color.FromArgb((byte)_randGen.Next(5, 220), (byte)_randGen.Next(0, 255), (byte)_randGen.Next(0, 255), (byte)_randGen.Next(0, 255)));

            Ellipse circle = new Ellipse()
            {
                Width = width,
                Height = height,
                Stroke = color,
                StrokeThickness = 2,
                Fill = color
            };

            Shapes.Add(new GrowingCircle(x, y, circle, range) { min_size = Math.Max(5, width - 10), max_size = width + 10 });

            _canvas.Children.Add(circle);
        }

        public void AddAudiobars(int width, int scale)
        {
            var color = new SolidColorBrush(Color.FromArgb((byte)_randGen.Next(100, 220), (byte)_randGen.Next(0, 255), (byte)_randGen.Next(0, 255), (byte)_randGen.Next(0, 255)));


            int init_x = 0;
            int init_y = 0;
            int offset = 0;
            int incBy = 50;

            for (int i = 0; i < 4000; i += incBy)
            {
                Rectangle rect = new Rectangle()
                {
                    Width = width,
                    Height = 5,
                    Stroke = color,
                    Fill = color,
                    StrokeThickness = 1,
                    VerticalAlignment = VerticalAlignment.Top
                };

                Shapes.Add(new AudioBar(init_x + width * offset + width * 2, init_y, rect, new FreqRange(i, i + incBy)) { scale = scale });

                if (i > 1000)
                {
                    incBy = 200;
                }
                else if (i > 3000)
                {
                    incBy = 400;
                }

                _canvas.Children.Add(rect);
                offset++;
            }

        }

        public void Update()
        {
            foreach (var item in Shapes)
            {
                item.Update(Results, RawBuffer);
            }
        }

        public void RedrawCircles(int circleCount, double max_X, double max_Y)
        {
            for (int i = 0; i < circleCount / 2; i++)
            {
                var size = _randGen.Next(8, 32);
                AddMovingCircle(_randGen.Next((int)max_X), _randGen.Next((int)max_Y), size, size);
            }

            var rangeSize = 50;
            var numRanges = 4_000 / rangeSize;
            for (int i = 0; i < numRanges; i++)
            {
                double scale = 0.75 + _randGen.NextDouble();
                for (int j = 0; j < circleCount / numRanges; j++)
                {
                    var size = _randGen.Next(64, 256);
                    AddGrowingCircle(_randGen.Next((int)max_X), _randGen.Next((int)max_Y), size, size, new FreqRange(rangeSize * i, rangeSize * i + rangeSize));
                }
            }
        }

        internal void Clear<T>() where T : VizShape
        {
            foreach (var item in Shapes)
            {
                if (item is T)
                {
                    _canvas.Children.Remove(item.element);
                    item.element = null;
                }
            }

            Shapes.RemoveAll(s => s is T);
        }

        internal void DrawWaveFormCircle()
        {
            var color = new SolidColorBrush(Color.FromArgb(255, (byte)_randGen.Next(0, 255), (byte)_randGen.Next(0, 255), (byte)_randGen.Next(0, 255)));

            Polyline line = new Polyline()
            {
                StrokeThickness = 3,
                Stroke = color,
                Points = new PointCollection(360),
            };

            DrawWaveFormCircle(new WaveFormCircle(CanvasUtil.Instance.Width / 2, CanvasUtil.Instance.Height / 2, line)
            {
                min_size = _randGen.Next(10, 50),
                max_size = _randGen.Next(100, (int)CanvasUtil.Instance.Height / 2),
                AngleSpeed = 0.001
            });
        }

        internal void DrawWaveFormCircle(WaveFormCircle circle)
        {
            _canvas.Children.Add(circle.element);
            Shapes.Add(circle);
        }

        internal void ClearAll()
        {
            foreach (var item in Shapes)
            {
                _canvas.Children.Remove(item.element);
                item.element = null;
            }

            Shapes.Clear();
        }
    }


}
