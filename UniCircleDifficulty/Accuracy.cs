using System;

using UniCircleTools.Beatmaps;

namespace UniCircleDifficulty
{
    /// <summary>
    /// Skill representing the difficulty of accurately tapping notes
    /// </summary>
    class Accuracy : Skill
    {
        // Rhythm complexity, od
        protected override double ExcertionDecayBase => 0.1;

        protected override double SkillMultiplier => 1;

        protected override double CalculateRawDiff()
        {
            throw new NotImplementedException();
        }
    }
}
