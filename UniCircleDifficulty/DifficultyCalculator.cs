using System;

using UniCircleTools;
using UniCircleTools.Beatmaps;

namespace UniCircleDifficulty
{
    public class DifficultyCalculator
    {
        private Beatmap _beatmap;
        private Mods _mods;
        private bool _calculated;

        private Aim _aim;
        private Speed _speed;
        private Accuracy _accuracy;
        private Reading _reading;

        public double AimDifficulty { get => _aim.Value; }
        public double SpeedDifficulty { get => _speed.Value; }
        public double AccuracyDifficulty { get => _accuracy.Value; }
        public double ReadingDifficulty { get => _reading.Value; }

        public double Difficulty {
            get
            {
                if (!_calculated)
                {
                    CalculateDifficulty();
                }
                return AimDifficulty + SpeedDifficulty + AccuracyDifficulty + ReadingDifficulty;
            }
        }

        public DifficultyCalculator(Beatmap beatmap, Mods mods = Mods.None)
        {
            _beatmap = beatmap;
            _mods = mods;
            _calculated = false;
            PreprocessBeatmap();
        }

        public void CalculateDifficulty()
        {
            // Calculates skill difficulties
            _aim = new Aim();
            _speed = new Speed();
            _accuracy = new Accuracy();
            _reading = new Reading();

            _aim.ProcessHitObjectSequence(_beatmap.HitObjects);
            _speed.ProcessHitObjectSequence(_beatmap.HitObjects);
            _accuracy.ProcessHitObjectSequence(_beatmap.HitObjects);
            _reading.ProcessHitObjectSequence(_beatmap.HitObjects);

            _calculated = true;
        }

        private void PreprocessBeatmap()
        {
            // Modify beatmap in alignment with map changing mods
        }
    }
}
