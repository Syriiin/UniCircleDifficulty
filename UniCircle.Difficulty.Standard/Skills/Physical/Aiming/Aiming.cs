using System;

using UniCircle.Difficulty.Skills.Physical;
using UniCircleTools;
using UniCircleTools.Beatmaps;

namespace UniCircle.Difficulty.Standard.Skills.Physical.Aiming
{
    /// <summary>
    /// Skill representing the difficulty of moving your cursor between notes
    /// </summary>
    public class Aiming : PhysicalSkill<AimPoint>
    {
        // Bonus constants
        public double AngleDiffWeight { get; set; } = 0.3;
        public double SteadyDiffWeight { get; set; } = 0.2;

        // Snappiness constants
        public double SnapThreshold { get; set; } = 100;
        public double SnapCurveHarshness { get; set; } = 0.3;  // Higher = quicker change

        // Shortcuts for readability
        private AimPoint AimPointA => GetDifficultyPoint(0);
        private AimPoint AimPointB => GetDifficultyPoint(1);
        private AimPoint AimPointC => GetDifficultyPoint(2);

        // Exertion decay rate
        public override double SpeedDecayBase { get; set; } = 0.15;
        public override double StaminaDecayBase { get; set; } = 0.3;

        // Exertion weights
        public override double SpeedWeight { get; set; } = 1;
        public override double StaminaWeight { get; set; } = 1;

        public override double SkillMultiplier { get; set; } = 1;

        public override void ProcessHitObject(HitObject hitObject)
        {
            if (hitObject is Spinner)
            {
                // Spinners are not considered in aim
                return;
            }

            double offset = hitObject.Time / Utils.ModClockRate(Mods);

            // Construct aim points from hitobject and call ProcessDifficultyPoint with them
            var aimPoint = new AimPoint
            {
                BaseObject = hitObject,
                DeltaTime = offset - AimPointA?.Offset ?? offset,
                Offset = offset,
                X = hitObject.X,
                Y = hitObject.Y,
                Radius = Utils.ModRadius(hitObject.Difficulty.CS, Mods)
            };

            if (Mods.HasFlag(Mods.HardRock))
            {
                aimPoint.Y = -aimPoint.Y + 384; // Flip notes (even though it technically doesnt matter since EVERYTHING is flipped)
            }

            ProcessDifficultyPoint(aimPoint);
        }

        protected override void UpdateDifficultyPoints(AimPoint aimPoint)
        {
            CurrentDiffPoints.Add(aimPoint);

            if (CurrentDiffPoints.Count == 4)
            {
                CurrentDiffPoints.RemoveAt(0);
            }
        }

        // Calculate the energy of a jump, that is, only concerning the distance between the objects
        protected override double CalculateEnergyExerted()
        {
            if (AimPointB == null)  // First object, thus no difficulty
            {
                return 0;
            }
            
            double distance = Utils.Distance(AimPointA, AimPointB);
            
            return distance;
        }

        // Calculate the degree to which the angle affects the difficulty of a jump
        // Multiplier of raw difficulty
        protected override double CalculateSemanticBonus()
        {
            if (AimPointC == null) // This is the second object in the map
            {
                // No angle difficulty, since there is no angle
                return 0;
            }
            
            double angle = Utils.Angle(AimPointC, AimPointB, AimPointA);
            if (double.IsNaN(angle))
            {
                return 0;
            }

            double prevDelay = AimPointB.DeltaTime;   // previous because between object B and C
            double snappiness = Snappiness(prevDelay);

            double angleDifficulty = AngleDifficulty(angle, snappiness) * AngleDiffWeight;
            double steadinessDifficulty = SteadinessDifficulty(snappiness) * SteadyDiffWeight;

            return angleDifficulty + steadinessDifficulty + CircleSizeDifficulty(AimPointA.Radius);
        }

        private double CircleSizeDifficulty(double radius)
        {
            return 52 / radius;
        }

        // Difficulty of angle depending on snappiness
        // Range: [0, 1]
        private double AngleDifficulty(double angle, double snappiness)
        {
            // Difficulty of angles depends on how they are played, wide angles are harder when snapping into, but opposite when flowing into
            // Higher snappiness will result in higher reward for high (more flat) angles, reverse for low snappiness
            return (snappiness * Math.Sin(angle - Math.PI / 2)) / 2 + 0.5;
        }

        // Difficulty of aiming notes steadily in time with their offsets.
        // Most relevent for spaced streams
        // Range: [0, 1]
        private double SteadinessDifficulty(double snappiness)
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
        private double Snappiness(double delay)
        {
            return Math.Tanh(SnapCurveHarshness * (delay - SnapThreshold));
        }
    }
}
