using System;

using UniCircleTools.Beatmaps;

namespace UniCircleDifficulty
{
    /// <summary>
    /// Skill representing the difficulty of moving your cursor between notes
    /// </summary>
    class Aim : Skill
    {
        // TODO:
        // - Slider support
        //      Add slider support, for slider tick aim, angle from slider end with last tick, distance and speed from slider end, etc..
        //      may cause problems with maps like big black where sliders can be tapped like circles, perhaps check for ticks
        // - Spacing changes?
        //      Contemplate spacing changes affecting the raw aim difficulty of a pattern, ie. same speed, but massive distance change.
        //      perhaps change anglediff to awkwardness and include spacing changes. would buff cutstreams/accelerating streams and decronstruction star triples
        // - Buff spaced streams
        //      There is added difficulty with spaced streams, as they require flowing which meaning they are harder to tap accuratly.
        //      This is not part of speed, nor accuracy, since the required skill (consistant movement) is aim.
        //      Also, since perfection of other skills is assumed, this means all notes are being tapped at their exact offset.
        //      Buff should be applied as part of the bonus difficulty for low snappiness. Same (similar) function can be used.
        // - Reform snappiness to use sigmoid
        //      Will also require either transforming to fit same range, or modifying bonus function.
        //      Modifying bonus function is prefered, since it will be reformed anyway.

        // Snappiness constants
        private const double snap_threshold = 100;
        private const double snap_leniency = 2.5;

        // Shortcuts for readability
        private HitObject HitObjectC => GetHitObject(2);
        private HitObject HitObjectB => GetHitObject(1);
        private HitObject HitObjectA => GetHitObject(0);

        // Excertion decay rate
        protected override double ExcertionDecayBase => 0.15;

        protected override double SkillMultiplier => 0.5;

        public override void ProcessHitObject(HitObject hitObject)
        {
            if (hitObject is Spinner)
            {
                // Spinners are not considered in aim
                return;
            }

            if (_currentObjects.Count == 3)
            {
                _currentObjects.RemoveAt(0);
            }

            _currentObjects.Add(hitObject);

            if (_currentObjects.Count == 1) // This is the first object in the map
            {
                // No aim difficulty, since we havent aimed anything yet
                return;
            }

            base.ProcessHitObject(hitObject);
        }

        // Calculate the raw difficulty of a jump, that is, only concerning the distance and time between the objects
        protected override double CalculateRawDiff()
        {
            // Average CS (to support possible lazer variable CS)
            double avgRadius = (HitObjectB.Radius + HitObjectA.Radius) / 2;
            // Ratio of distance to CS
            double distanceRatio = Utils.Distance(HitObjectB, HitObjectA) / avgRadius;
            // Normalised distance at radius 52
            double distance = distanceRatio * 52;

            if (distance == 0)
            {
                // No movement means no aim (although there is still stacking but that can be ignored for the most part)
                return 0;
            }

            double delay = HitObjectA.Time - HitObjectB.Time;

            return distance / delay;
        }

        // Calculate the degree to which the angle affects the difficulty of a jump
        // Range: [1, 2]
        protected override double CalculateBonusDiff()
        {
            if (HitObjectC == null) // This is the second object in the map
            {
                // No angle difficulty, since there is no angle
                return 0;
            }
            
            double angle = Utils.Angle(HitObjectC, HitObjectB, HitObjectA);
            if (double.IsNaN(angle))
            {
                return 0;
            }

            double prevDelay = HitObjectB.Time - HitObjectC.Time;   // previous because between object A and B
            double snappiness = Snappiness(prevDelay);

            // Difficulty of angles depends on how they are played, wide angles are harder when snapping into, but opposite when flowing into
            // Higher snappiness will result in higher reward for high (more flat) angles, reverse for low snappiness
            double angleDifficulty = (angle * snappiness - Math.PI * (0.5 * snappiness - 0.5)) / Math.PI;

            return 1 + angleDifficulty * 0.3;
        }

        // Estimate the degree of snappiness in the aim between two objects based on delay. -1 = flow, 1 = snap
        // Different players will play different patterns with more less snappiness, but generally it follows a curve
        // Range: [-1, 1]
        private static double Snappiness(double delay)
        {
            return 2 * Math.Atan(snap_leniency * (delay - snap_threshold)) / Math.PI;
        }
    }
}
