using System;

using UniCircleTools;
using UniCircleTools.Beatmaps;

using UniCircleDifficulty.Skills.Physical.Aiming;
using UniCircleDifficulty.Skills.Physical.Clicking;
using UniCircleDifficulty.Skills.Reading;

namespace UniCircleDifficulty
{
    public class DifficultyCalculator
    {
        public Beatmap Beatmap { get; private set; }
        private bool _calculated;

        public Aiming Aiming { get; }
        public Clicking Clicking { get; }
        public Reading Reading { get; }

        public double Difficulty {
            get
            {
                if (!_calculated)
                {
                    CalculateDifficulty();
                }
                return Aiming.Value + Clicking.Value + Reading.Value;
            }
        }

        public DifficultyCalculator(Beatmap beatmap, Mods mods = Mods.None)
        {
            Beatmap = beatmap;
            Aiming = new Aiming(mods);
            Clicking = new Clicking(mods);
            Reading = new Reading(mods);
            _calculated = false;
        }

        public void CalculateDifficulty()
        {
            // Calculates skill difficulties

            Aiming.ProcessHitObjectSequence(Beatmap.HitObjects);
            Clicking.ProcessHitObjectSequence(Beatmap.HitObjects);
            Reading.ProcessHitObjectSequence(Beatmap.HitObjects);

            _calculated = true;
        }
    }
}
