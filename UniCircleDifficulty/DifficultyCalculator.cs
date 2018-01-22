using System;

using UniCircleTools;
using UniCircleTools.Beatmaps;

using UniCircleDifficulty.Skills;
using UniCircleDifficulty.Skills.Physical.Aiming;
using UniCircleDifficulty.Skills.Physical.Clicking;
using UniCircleDifficulty.Skills.Reading;

namespace UniCircleDifficulty
{
    public class DifficultyCalculator
    {
        private Beatmap _beatmap;
        private bool _calculated;

        private Aim _aim;
        private Clicking _clicking;
        private Reading _reading;

        public double AimDifficulty { get => _aim.Value; }
        public double SpeedDifficulty { get => _clicking.Value; }
        public double ReadingDifficulty { get => _reading.Value; }

        public double Difficulty {
            get
            {
                if (!_calculated)
                {
                    CalculateDifficulty();
                }
                return AimDifficulty + SpeedDifficulty + ReadingDifficulty;
            }
        }

        public DifficultyCalculator(Beatmap beatmap, Mods mods = Mods.None)
        {
            _beatmap = beatmap;
            _aim = new Aim(mods);
            _clicking = new Clicking(mods);
            _reading = new Reading(mods);
            _calculated = false;
        }

        public void CalculateDifficulty()
        {
            // Calculates skill difficulties

            _aim.ProcessHitObjectSequence(_beatmap.HitObjects);
            _clicking.ProcessHitObjectSequence(_beatmap.HitObjects);
            //_reading.ProcessHitObjectSequence(_beatmap.HitObjects);

            _calculated = true;
        }
    }
}
