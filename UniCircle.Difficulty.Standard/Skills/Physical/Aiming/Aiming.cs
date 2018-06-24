using UniCircle.Difficulty.Skills.Physical.Dimensional;
using UniCircleTools;
using UniCircleTools.Beatmaps;

namespace UniCircle.Difficulty.Standard.Skills.Physical.Aiming
{
    /// <summary>
    /// Skill representing the difficulty of moving your cursor between notes
    /// </summary>
    public class Aiming : DimensionalSkill<AimPoint>
    {
        // Shortcuts for readability
        private AimPoint LatestAimPoint => GetDifficultyPoint(0);

        // Exertion recovery rate
        public override double MaxSpeedRecoveryRate { get; set; } = 0.9;
        public override double MaxStaminaRecoveryRate { get; set; } = 0.1;
        public override double ExertionNormaliser { get; set; } = 1;

        // Exertion weights
        public override double SpeedWeight { get; set; } = 0.0005;
        public override double StaminaWeight { get; set; } = 0.00005;

        public override double SkillMultiplier { get; set; } = 4;

        public override double SnapForceThreshold { get; set; } = 5;
        public override double FlowForceThreshold { get; set; } = 13;
        public override double SnapForceVolatilityRecoveryRate { get; set; } = 0.9;

        public override void ProcessHitObject(HitObject hitObject)
        {
            if (hitObject is Spinner)
            {
                // Spinners are not considered in aim
                return;
            }

            double offset = hitObject.Time / Utils.ModClockRate(Mods);

            Vector position;
            if (Mods.HasFlag(Mods.HardRock))
            {
                // Flip notes (even though it technically doesnt matter since EVERYTHING is flipped)
                position = new Vector(hitObject.X, -hitObject.Y + 384);
            }
            else
            {
                position = new Vector(hitObject.X, hitObject.Y);
            }

            // Construct aim points from hitobject and call ProcessDifficultyPoint with them
            var aimPoint = new AimPoint
            {
                BaseObject = hitObject,
                DeltaTime = offset - LatestAimPoint?.Offset ?? offset,
                Offset = offset,
                Position = position,
                IncomingForce = position - (LatestAimPoint?.Position ?? position),
                Radius = Utils.ModRadius(hitObject.Difficulty.CS, Mods),
                TargetErrorRange = Utils.ModRadius(hitObject.Difficulty.CS, Mods)
            };

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
    }
}
