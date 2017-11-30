using System;

using UniCircleTools.Beatmaps;

namespace UniCircleDifficulty
{
    class Speed : Skill
    {
        // Shortcuts for readability
        private HitObject HitObjectB => GetHitObject(1);
        private HitObject HitObjectA => GetHitObject(0);

        // Excertion decay rate
        protected override double ExcertionDecayBase => 0.3;

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

            _currentObjects.Add(hitObject);

            if (_currentObjects.Count == 1) // This is the first object in the map
            {
                // No speed difficulty, since we have only tapped 1 note
                return;
            }

            base.ProcessHitObject(hitObject);
        }

        protected override double CalculateRawDiff()
        {
            return 1.0 / (HitObjectA.Time - HitObjectB.Time);
        }

        protected override double CalculateBonusDiff()
        {
            // In ppv2, lower time is worth more for higher spaced objects. 
            // This can be done in bonus difficulty if desired, but perhaps shouldnt be, since its somewhat regarding aim.
            // But at the same time, it is only using aim to define another value, same as snappiness. Perhaps the same method should be done here.
            return 1;
        }
    }
}
