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

        private bool showAudioBars = false;
        private bool showWaveformCircle = true;

        private double audioBarMax = 4000;
        private double audioBarMin = 0;
        private string audioBarMarginInput = "";
        private string audioBarIntervalInput = "";

        public double TimerUpdateInterval { get => timerUpdateInterval; set => timerUpdateInterval = Math.Round(value, 0); }


        public double MovingCircleCount { get => movingCircleCount; set => movingCircleCount = Math.Round(value, 0); }


        public bool ShowWaveformCircle { get => showWaveformCircle; set => showWaveformCircle = value; }
        public double WaveFormCircleCount { get => waveFormCircleCount; set => waveFormCircleCount = Math.Round(value, 0); }


        public bool ShowAudioBars { get => showAudioBars; set => showAudioBars = value; }
        public int AudioBarFreqInterval {
            get
            {
                if (!string.IsNullOrWhiteSpace(AudioBarIntervalInput))
                {
                    int.TryParse(AudioBarIntervalInput.Trim(), out int m);
                    return m;
                }

                return 50;
            }
        }
        public double AudioBarMax { get => audioBarMax; set => audioBarMax = value; }
        public double AudioBarMin { get => audioBarMin; set => audioBarMin = value; }

        public double AudioBarMarginLeft
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(AudioBarMarginInput))
                {
                    var split = AudioBarMarginInput.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                    double.TryParse(split[0], out double m);
                    return m;
                }

                return 0;
            }
        }

        public double AudioBarMarginRight
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(AudioBarMarginInput))
                {
                    var split = AudioBarMarginInput.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

                    if (split.Length > 1)
                    {
                        double.TryParse(split[1], out double m);
                        return m;
                    }
                }

                return 0;
            }
        }

        public string AudioBarMarginInput { get => audioBarMarginInput; set => audioBarMarginInput = value; }

        public string AudioBarIntervalInput { get => audioBarIntervalInput; set => audioBarIntervalInput = value; }
    }
}
