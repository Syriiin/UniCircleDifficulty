using System;

using UniCircleTools.Beatmaps;

namespace UniCircleDifficulty
{
    /// <summary>
    /// Skill representing the difficulty of keeping up with tapping speed of notes
    /// </summary>
    class Speed : Skill
    {
        // Shortcuts for readability
        private HitObject HitObjectB => GetHitObject(1);
        private HitObject HitObjectA => GetHitObject(0);

        // Excertion decay rate
        protected override double ExcertionDecayBase => 0.3;

        protected override double SkillMultiplier => 5;

        public override void ProcessHitObject(HitObject hitObject)
        {
            if (hitObject is Spinner)
            {
                // Spinners are not considered in speed
                return;
            }

            if (_currentObjects.Count == 2)
            {
                _currentObjects.RemoveAt(0);
            }

            base.ProcessHitObject(hitObject);
        }

        protected override double CalculateRawDiff()
        {
            // In ppv2, higher spaced objects are worth more to reward spaced streams.
            // This can is really part of aim, and thus speed is not concerned with it.
            return 1.0 / (HitObjectA.Time - HitObjectB.Time);
        }
    }
}
