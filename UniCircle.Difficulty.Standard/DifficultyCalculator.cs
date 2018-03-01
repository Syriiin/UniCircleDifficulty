using UniCircle.Difficulty.Standard.Skills.Physical.Aiming;
using UniCircle.Difficulty.Standard.Skills.Physical.Clicking;
using UniCircle.Difficulty.Standard.Skills.Reading;

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
            Skills.Add(Aiming);
            Skills.Add(Clicking);
            Skills.Add(Reading);
        }
    }
}
