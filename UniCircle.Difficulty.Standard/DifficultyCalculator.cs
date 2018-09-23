using UniCircle.Difficulty.Skills;
using UniCircle.Difficulty.Standard.Skills.Physical.Aiming;
using UniCircle.Difficulty.Standard.Skills.Physical.Clicking;
using UniCircle.Difficulty.Standard.Skills.Reading;

using UniCircleTools;
using UniCircleTools.Beatmaps;

namespace UniCircle.Difficulty.Standard
{
    /// <summary>
    /// Difficulty calculator for osu!standard
    /// </summary>
    public class DifficultyCalculator : Difficulty.DifficultyCalculator
    {
        public DifficultyCalculator()
        {
            Skills.AddRange(new ISkill[]
            {
                new Aiming(),
                new Clicking(),
                new Reading(),
            });
        }

        /// <inheritdoc />
        protected override HitObject HitObjectWithMods(HitObject baseHitObject, Mods mods)
        {
            throw new System.NotImplementedException();
        }
    }
}
