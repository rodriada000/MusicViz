using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using FftSharp;
using System.Linq;

namespace MusicViz.Classes
{
    /// <summary>
    /// Performs FFT calculations once <see cref="fftBuffer"/> is full
    /// </summary>
    /// <remarks>
    /// Based on this class by NAudio https://github.com/SjB/NAudio/blob/master/NAudioWpfDemo/AudioPlaybackDemo/SampleAggregator.cs
    /// but modified to use FftSharp
    /// </remarks>
    public class SampleAggregator
    {
        public const int SAMPLE_RATE = 48_000;
        // volume
        public event EventHandler<MaxSampleEventArgs> MaximumCalculated;
        private float maxValue;
        private float minValue;
        public int NotificationCount { get; set; }
        int count;

        // FFT
        public event EventHandler<FftEventArgs> FftCalculated;
        public bool PerformFFT { get; set; }
        private double[] fftBuffer;
        private FftEventArgs fftArgs;
        private int fftPos;
        private int fftLength;
        private int m;

        public SampleAggregator(int fftLength = 1024)
        {
            if (!IsPowerOfTwo(fftLength))
            {
                throw new ArgumentException("FFT Length must be a power of two");
            }
            this.m = (int)Math.Log(fftLength, 2.0);
            this.fftLength = fftLength;
            this.fftBuffer = new double[fftLength];
            this.fftArgs = new FftEventArgs();
        }

        bool IsPowerOfTwo(int x)
        {
            return (x & (x - 1)) == 0;
        }

        public void Reset()
        {
            count = 0;
            maxValue = minValue = 0;
        }

        public void Add(float value)
        {
            if (PerformFFT && FftCalculated != null)
            {
                fftBuffer[fftPos] = (double)value;
                fftPos++;

                if (fftPos >= fftBuffer.Length)
                {
                    fftPos = 0;

                    var window = new FftSharp.Windows.Hanning();
                    double[] windowed = window.Apply(fftBuffer);

                    fftArgs.Result = FftSharp.Transform.FFT(windowed);
                    fftArgs.Power = FftSharp.Transform.FFTpower(windowed);
                    fftArgs.Magnitude = FftSharp.Transform.FFTmagnitude(windowed);
                    fftArgs.Frequencies = FftSharp.Transform.FFTfreq(SAMPLE_RATE, fftArgs.Power.Length).Select(f => f * 2).ToArray(); // multiplying the frequency by 2 seems to give me the correct range... ¯\_(ツ)_/¯

                    FftCalculated(this, fftArgs);
                }
            }

            maxValue = Math.Max(maxValue, value);
            minValue = Math.Min(minValue, value);
            count++;
            if (count >= NotificationCount && NotificationCount > 0)
            {
                if (MaximumCalculated != null)
                {
                    MaximumCalculated(this, new MaxSampleEventArgs(minValue, maxValue));
                }
                Reset();
            }
        }
    }

    public class MaxSampleEventArgs : EventArgs
    {
        public MaxSampleEventArgs(float minValue, float maxValue)
        {
            this.MaxSample = maxValue;
            this.MinSample = minValue;
        }
        public float MaxSample { get; private set; }
        public float MinSample { get; private set; }
    }

    public class FftEventArgs : EventArgs
    {
        public FftEventArgs()
        {
        }

        public FftEventArgs(Complex[] result)
        {
            this.Result = result;
        }

        public Complex[] Result { get; set; }
        public double[] Magnitude { get; set; }
        public double[] Power { get; set; }
        public double[] Frequencies { get; set; }
    }
}
