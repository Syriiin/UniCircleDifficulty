using UniCircle.Difficulty.Skills.Physical.Binary;
using UniCircleTools.Beatmaps;

namespace UniCircle.Difficulty.Standard.Skills.Physical.Clicking
{
    /// <summary>
    /// Skill representing the difficulty of keeping up with tapping speed of notes
    /// </summary>
    public class Clicking : BinarySkill<ClickPoint>
    {
        // Shortcuts for readability
        private ClickPoint ClickPointA => GetDifficultyPoint(0);

        // Exertion recovery rate
        public override double MaxSpeedRecoveryRate { get; set; } = 0.9;
        public override double MaxStaminaRecoveryRate { get; set; } = 0.1;
        public override double ExertionNormaliser { get; set; } = 150;

        // Exertion weights
        public override double SpeedWeight { get; set; } = 0.5;
        public override double StaminaWeight { get; set; } = 0.05;

        public override double SkillMultiplier { get; set; } = 0.32;

        public override void ProcessHitObject(HitObject hitObject)
        {
            if (hitObject is Spinner)
            {
                // Spinners are not considered in speed
                return;
            }

            double offset = hitObject.Time / Utils.ModClockRate(Mods);

            // Construct click point from hitobject and call ProcessDifficultyPoint with them
            var clickPoint = new ClickPoint
            {
                BaseObject = hitObject,
                DeltaTime = offset - ClickPointA?.Offset ?? offset,
                Offset = offset
            };

            ProcessDifficultyPoint(clickPoint);
        }

        protected override void UpdateDifficultyPoints(ClickPoint clickPoint)
        {
            // Add diffPoint to currentDiffPoints
            CurrentDiffPoints.Add(clickPoint);

            // Update pool
            if (CurrentDiffPoints.Count == 3)
            {
                CurrentDiffPoints.RemoveAt(0);
            }
        }
    }
}
