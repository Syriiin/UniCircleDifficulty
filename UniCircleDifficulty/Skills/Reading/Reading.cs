using System;

using UniCircleTools;
using UniCircleTools.Beatmaps;

namespace UniCircleDifficulty.Skills.Reading
{
    /// <summary>
    /// Skill representing the difficulty of identifying rhythmic and visual patterns in notes
    /// </summary>
    class Reading : Skill
    {
        // Density, distance, overlap, visual mods
        
        // Reading doesn't really have lingering difficulty. If anything, it gets easier since you're getting used to the hard reading.
        protected override double ExcertionDecayBase => 0;

        protected override double SkillMultiplier => 1;

        protected override double CalculateRawDiff()
        {
            throw new NotImplementedException();
        }

        public Reading(Mods mods) : base(mods) { }
    }
}
