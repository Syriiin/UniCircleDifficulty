﻿using System;

using UniCircleTools;
using UniCircleTools.Beatmaps;

namespace UniCircleDifficulty.Skills.Aiming
{
    /// <summary>
    /// Skill representing the difficulty of moving your cursor between notes
    /// </summary>
    class Aim : Skill
    {
        // TODO:
        // - Slider support
        //      Add slider support, for slider tick aim, angle from slider end with last tick, distance and speed from slider end, etc..
        //      May cause problems with maps like big black where sliders can be tapped like circles, perhaps check for ticks instead.
        // - Spacing changes?
        //      Create (awkwardness?) to account for massive time-distance equality changes.
        //      Would buff cutstreams/accelerating streams, deconstruction star style triples, worldwide choppers hard part, etc...

        // Bonus constants
        private const double angle_diff_weight = 0.3;
        private const double steady_diff_weight = 0.2;

        // Snappiness constants
        private const double snap_threshold = 100;
        private const double snap_curve_harshness = 0.3;  // Higher = quicker change

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
        // Multiplier of raw difficulty
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

            double prevDelay = HitObjectB.Time - HitObjectC.Time;   // previous because between object B and C
            double snappiness = Snappiness(prevDelay);

            double angleDifficulty = AngleDifficulty(angle, snappiness);
            double steadinessDifficulty = SteadinessDifficulty(snappiness);

            return 1 + 0.3 * angleDifficulty + 0.2 * steadinessDifficulty;
        }

        // Difficulty of angle depending on snappiness
        // Range: [0, 1]
        private static double AngleDifficulty(double angle, double snappiness)
        {
            // Difficulty of angles depends on how they are played, wide angles are harder when snapping into, but opposite when flowing into
            // Higher snappiness will result in higher reward for high (more flat) angles, reverse for low snappiness
            return (angle * snappiness - Math.PI * (0.5 * snappiness - 0.5)) / Math.PI;
        }

        // Difficulty of aiming notes steadily in time with their offsets.
        // Most relevent for spaced streams
        // Range: [0, 1]
        private static double SteadinessDifficulty(double snappiness)
        {
            // High snappiness give no bonus
            // Low snappiness (streams) gain a bonus based on (spacing?)
            return Math.Pow(snappiness - 1, 2) / 4;

            // PPv2 buffed spaced streams proportionally to tapping speed,
            //  whereas we are buffing them proportionally to aim speed.
            // This may need to be changed later to scale with tapping speed.
        }

        // Estimate the degree of snappiness in the aim between two objects based on delay. -1 = flow, 1 = snap
        // Different players will play different patterns with more less snappiness, but generally it follows a curve
        // Range: [-1, 1]
        private static double Snappiness(double delay)
        {
            return Math.Tanh(snap_curve_harshness * (delay - snap_threshold));
        }

        public Aim(Mods mods) : base(mods) { }
    }
}
