using System;

using UniCircleTools;
using UniCircleTools.Beatmaps;

using UniCircleDifficulty.Skills;
using UniCircleDifficulty.Skills.Aiming;
using UniCircleDifficulty.Skills.Clicking;
using UniCircleDifficulty.Skills.Reading;

namespace UniCircleDifficulty
{
    public class DifficultyCalculator
    {
        private Beatmap _beatmap;
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
            _aim = new Aim(mods);
            _speed = new Speed(mods);
            _accuracy = new Accuracy(mods);
            _reading = new Reading(mods);
            _calculated = false;
        }

        public void CalculateDifficulty()
        {
            // Calculates skill difficulties

            _aim.ProcessHitObjectSequence(_beatmap.HitObjects);
            _speed.ProcessHitObjectSequence(_beatmap.HitObjects);
            //_accuracy.ProcessHitObjectSequence(_beatmap.HitObjects);
            //_reading.ProcessHitObjectSequence(_beatmap.HitObjects);

            _calculated = true;
        }
    }
}
