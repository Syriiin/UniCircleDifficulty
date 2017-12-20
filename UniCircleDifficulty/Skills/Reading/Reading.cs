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

        public override void ProcessHitObject(HitObject hitObject)
        {
            // Construct diff points from hitobject and call ProcessDifficultyPoint with them
            throw new NotImplementedException();
        }

        protected override void UpdateDifficultyPoints(DifficultyPoint diffPoint)
        {
            // Add diffPoint to currentDiffPoints
            _currentDiffPoints.Add(diffPoint);

            // Update pool
            throw new NotImplementedException();
        }

        protected override double CalculateRawDiff()
        {
            throw new NotImplementedException();
        }

        public Reading(Mods mods) : base(mods) { }
    }
}
