using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicViz.ViewModels
{
    internal class MainViewModel
    {
        private double timerUpdateInterval = 20;
        private double movingCircleCount = 100;
        private double waveFormCircleCount = 2;

        public double TimerUpdateInterval { get => timerUpdateInterval; set => timerUpdateInterval = Math.Round(value, 0); }

        public double MovingCircleCount { get => movingCircleCount; set => movingCircleCount = Math.Round(value, 0); }

        public double WaveFormCircleCount { get => waveFormCircleCount; set => waveFormCircleCount = Math.Round(value, 0); }
    }
}
