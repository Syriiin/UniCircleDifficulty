using System;

using UniCircleDifficulty.Skills.Physical;
using UniCircleTools;
using UniCircleTools.Beatmaps;

namespace UniCircle.Difficulty.Standard.Skills.Physical.Aiming
{
    /// <summary>
    /// Skill representing the difficulty of moving your cursor between notes
    /// </summary>
    public class Aiming : PhysicalSkill<AimPoint>
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
        private AimPoint AimPointA => GetDifficultyPoint(0);
        private AimPoint AimPointB => GetDifficultyPoint(1);
        private AimPoint AimPointC => GetDifficultyPoint(2);

        // Exertion decay rate
        protected override double SpeedDecayBase => 0.15;
        protected override double StaminaDecayBase => 0.3;

        // Exertion weights
        protected override double SpeedWeight => 1;
        protected override double StaminaWeight => 1;

        protected override double SkillMultiplier => 1;

        public override void ProcessHitObject(HitObject hitObject)
        {
            if (hitObject is Spinner)
            {
                // Spinners are not considered in aim
                return;
            }

            double offset = hitObject.Time / Utils.ModClockRate(_mods);

            // Construct aim points from hitobject and call ProcessDifficultyPoint with them
            AimPoint aimPoint = new AimPoint
            {
                DeltaTime = offset - AimPointB?.Offset ?? offset,
                Offset = offset,
                X = hitObject.X,
                Y = hitObject.Y,
                Radius = Utils.ModRadius(hitObject.Difficulty.CS, _mods)
            };

            if (_mods.HasFlag(Mods.HardRock))
            {
                aimPoint.Y = -aimPoint.Y + 384; // Flip notes (even though it technically doesnt matter since EVERYTHING is flipped)
            }

            // TODO: Process sliderticks into aim points when they are implemented

            ProcessDifficultyPoint(aimPoint);
        }

        protected override void UpdateDifficultyPoints(AimPoint aimPoint)
        {
            _currentDiffPoints.Add(aimPoint);

            if (_currentDiffPoints.Count == 4)
            {
                _currentDiffPoints.RemoveAt(0);
            }
        }

        // Calculate the energy of a jump, that is, only concerning the distance between the objects
        protected override double CalculateEnergyExerted()
        {
            if (AimPointB == null)  // First object, thus no difficulty
            {
                return 0;
            }
            
            // Normalised distance at radius 52
            double distance = Utils.NormalisedDistance(AimPointA, AimPointB);
            
            return distance;
        }

        // Calculate the degree to which the angle affects the difficulty of a jump
        // Multiplier of raw difficulty
        protected override double CalculateSemanticBonus()
        {
            if (AimPointC == null) // This is the second object in the map
            {
                // No angle difficulty, since there is no angle
                return 1;
            }
            
            double angle = Utils.Angle(AimPointC, AimPointB, AimPointA);
            if (double.IsNaN(angle))
            {
                return 1;
            }

            double prevDelay = AimPointB.DeltaTime;   // previous because between object B and C
            double snappiness = Snappiness(prevDelay);

            double angleDifficulty = AngleDifficulty(angle, snappiness) * angle_diff_weight;
            double steadinessDifficulty = SteadinessDifficulty(snappiness) * steady_diff_weight;

            return 1 + angleDifficulty + steadinessDifficulty;
        }

        // Difficulty of angle depending on snappiness
        // Range: [0, 1]
        private static double AngleDifficulty(double angle, double snappiness)
        {
            // Difficulty of angles depends on how they are played, wide angles are harder when snapping into, but opposite when flowing into
            // Higher snappiness will result in higher reward for high (more flat) angles, reverse for low snappiness
            return (snappiness * Math.Sin(angle - Math.PI / 2)) / 2 + 0.5;
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
    }
}
