using System;

using UniCircleTools.Beatmaps;

namespace UniCircleDifficulty.Skills.Clicking
{
    /// <summary>
    /// Skill representing the difficulty of accurately tapping notes
    /// </summary>
    class Accuracy : Skill
    {
        protected override double ExcertionDecayBase => 0.1;

        protected override double SkillMultiplier => 1;

        public override void ProcessHitObject(HitObject hitObject)
        {
            if (hitObject is Spinner)
            {
                // Spinners are not considered in accuracy
                return;
            }

            // TODO: Remove finished objects from _currentObjects here

            base.ProcessHitObject(hitObject);
        }

        protected override double CalculateRawDiff()
        {
            // Raw difficulty is hit window for 300 (everything else is bad for accuracy)
            // Affected by ScoreV2, sliders (especially overlapping hitwindows)
            return 0;
        }

        protected override double CalculateBonusDiff()
        {
            // Bonus is rhythm complexity
            return 0;
        }
    }
}
