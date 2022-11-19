using FftSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicViz.Classes
{
    public class FFTResult
    {
        public const int SAMPLE_RATE = 48_000;


        public FFTResult(FftEventArgs args)
        {
            FFTPower = args.Power;
            FFTFreq = args.Frequencies;
            FFTMagnitude = args.Magnitude;
            FFTResults = args.Result;
        }

        public double PeakFreq { get; set; }
        public double PeakPower { get; set; }
        public double[] FFTPower { get; set; }
        public double[] FFTFreq { get; set; }
        public double[] FFTMagnitude { get; set; }
        public Complex[] FFTResults { get; set; }
        public double PeakMagnitude { get; internal set; }
    }
}
