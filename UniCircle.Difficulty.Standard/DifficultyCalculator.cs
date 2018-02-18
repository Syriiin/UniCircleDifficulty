using System;

using UniCircle.Difficulty;

using UniCircle.Difficulty.Standard.Skills.Physical.Aiming;
using UniCircle.Difficulty.Standard.Skills.Physical.Clicking;
using UniCircle.Difficulty.Standard.Skills.Visual;

namespace UniCircle.Difficulty.Standard
{
    public class DifficultyCalculator : Difficulty.DifficultyCalculator
    {
        public Aiming Aiming { get; }
        public Clicking Clicking { get; }
        public Reading Reading { get; }

        public DifficultyCalculator()
        {
            Aiming = new Aiming();
            Clicking = new Clicking();
            Reading = new Reading();
            _skills.Add(Aiming);
            _skills.Add(Clicking);
            _skills.Add(Reading);
        }
    }
}
