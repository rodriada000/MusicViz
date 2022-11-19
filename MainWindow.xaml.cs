using MusicViz.Classes;
using MusicViz.Shapes;
using MusicViz.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MusicViz
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        private readonly MainViewModel ViewModel;
        
        private NAudio.Wave.WasapiLoopbackCapture wvin;
        private float[] lastBuffer;
        private SampleAggregator aggregator;
        
        private VizManager drawer;
        private DispatcherTimer _timer;


        private object bufferLock = new object();


        public MainWindow()
        {
            InitializeComponent();

            ViewModel = new MainViewModel();
            this.DataContext = ViewModel;

            CanvasUtil.SetCanvas(MainCanvas);

            drawer = new VizManager(MainCanvas);
            _timer = new DispatcherTimer(DispatcherPriority.Normal, this.Dispatcher);
            _timer.Interval = TimeSpan.FromMilliseconds(ViewModel.TimerUpdateInterval);
            _timer.Tick += _timer_Tick;
            _timer.Start();

            toolBarRow.MaxHeight = 0;


            aggregator = new SampleAggregator(4096);
            aggregator.NotificationCount = 882;
            aggregator.PerformFFT = true;

            aggregator.FftCalculated += Aggregator_FftCalculated;
            aggregator.MaximumCalculated += Aggregator_MaximumCalculated;


            NewWaveCapture();
        }

        private void Aggregator_MaximumCalculated(object? sender, MaxSampleEventArgs e)
        {
            //Debug.WriteLine($"Min,Max = {e.MinSample},{e.MaxSample}");
        }

        private void Aggregator_FftCalculated(object? sender, FftEventArgs e)
        {
            UpdateFftResults(e);
        }

        public void UpdateFftResults(FftEventArgs fftResults)
        {
            double peakFreq = 0;
            double peakPower = 0;
            double peakMag = 0;
            int indexOfPeak = 0;

            for (int i = 0; i < fftResults.Magnitude.Length; i++)
            {
                if (fftResults.Magnitude[i] > peakMag)
                {
                    peakPower = fftResults.Power[i];
                    peakMag = fftResults.Magnitude[i];
                    peakFreq = fftResults.Frequencies[i];
                    indexOfPeak = i;
                }
            }


            drawer.Results = new FFTResult(fftResults)
            {
                PeakFreq = peakFreq,
                PeakPower = peakPower,
                PeakMagnitude = peakMag
            };
        }

        private void _timer_Tick(object? sender, EventArgs e)
        {
            if (_timer.Interval.TotalMilliseconds != ViewModel.TimerUpdateInterval)
            {
                _timer.Interval = TimeSpan.FromMilliseconds(ViewModel.TimerUpdateInterval);
                return;
            }


            if (drawer.Results?.FFTPower != null && drawer.Results?.FFTPower?.Length > 16)
            {
                try
                {
                    if (WpfPlot1.IsVisible)
                    {
                        WpfPlot1.Plot.Clear();
                        WpfPlot1.Plot.AddScatterLines(drawer.Results.FFTFreq, drawer.Results.FFTMagnitude);
                        WpfPlot1.Plot.SetAxisLimitsY(0, 0.5);
                        WpfPlot1.Plot.SetAxisLimitsX(0, 4000);
                        WpfPlot1.Refresh();
                    }
                }
                catch (Exception)
                {
                }
            }

            drawer.Update();

        }

        public void NewWaveCapture()
        {
            wvin?.Dispose();
            wvin = new NAudio.Wave.WasapiLoopbackCapture();
            wvin.DataAvailable += OnDataAvailable;

            wvin.StartRecording();
        }

        public void StopWaveCapture()
        {
            if (wvin != null)
            {
                wvin.StopRecording();
                wvin.DataAvailable -= OnDataAvailable;
                wvin.Dispose();
                wvin = null;
            }
        }


        private void OnDataAvailable(object sender, NAudio.Wave.WaveInEventArgs e)
        {
            if (e.BytesRecorded == 0)
                return;

            int bytesPerSample = (wvin.WaveFormat.BitsPerSample / 8) * wvin.WaveFormat.Channels;
            int samplesRecorded = (e.BytesRecorded / bytesPerSample) * wvin.WaveFormat.Channels;

            lock (bufferLock)
            {
                if (lastBuffer is null || lastBuffer.Length != e.BytesRecorded)
                    lastBuffer = new float[samplesRecorded];

                int k = 0;
                for (int i = 0; i < e.BytesRecorded - bytesPerSample; i += bytesPerSample)
                {
                    for (int j = 0; j < wvin.WaveFormat.Channels; j++)
                    {
                        //average the two channels
                        lastBuffer[k] = BitConverter.ToSingle(e.Buffer, i + sizeof(float) * j);
                        lastBuffer[k] += BitConverter.ToSingle(e.Buffer, i + sizeof(float) * (j+ 1));
                        lastBuffer[k] /= 2;
                        aggregator.Add(lastBuffer[k]);
                        k++;
                    }
                }

                drawer.RawBuffer = lastBuffer;
            }

        }

        private void MainCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            Redraw();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.C)
            {
                toolBarRow.MaxHeight = toolBarRow.MaxHeight > 0 ? 0 : 64;
            }
            else if (e.Key == Key.G)
            {
                WpfPlot1.Visibility = WpfPlot1.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            }

        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (drawer == null)
            {
                return;
            }

            drawer.Clear<MovingCircle>();
            drawer.RedrawCircles((int)ViewModel.MovingCircleCount, MainCanvas.ActualWidth, MainCanvas.ActualWidth);
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Redraw();
        }

        private void Redraw()
        {
            drawer?.ClearAll();
            drawer?.RedrawCircles((int)ViewModel?.MovingCircleCount, MainCanvas.ActualWidth, MainCanvas.ActualWidth);

            for (int i = 0; i < ViewModel?.WaveFormCircleCount; i++)
            {
                drawer?.DrawWaveFormCircle();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            StopWaveCapture();

            _timer.Stop();
            _timer.Tick -= _timer_Tick;
            _timer = null;

            aggregator.FftCalculated -= Aggregator_FftCalculated;
            aggregator.MaximumCalculated -= Aggregator_MaximumCalculated;
            aggregator = null;

        }

        private void waveFormSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            drawer?.Clear<WaveFormCircle>();

            for (int i = 0; i < ViewModel?.WaveFormCircleCount; i++)
            {
                drawer?.DrawWaveFormCircle();
            }
        }
    }


}
