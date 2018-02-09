using System;

using UniCircleDifficulty;

using UniCircle.Difficulty.Standard.Skills.Physical.Aiming;
using UniCircle.Difficulty.Standard.Skills.Physical.Clicking;
using UniCircle.Difficulty.Standard.Skills.Visual;

namespace UniCircle.Difficulty.Standard
{
    public class Calculator : DifficultyCalculator
    {
        public Aiming Aiming { get; }
        public Clicking Clicking { get; }
        public Reading Reading { get; }

        public Calculator()
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
