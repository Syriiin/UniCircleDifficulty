using UniCircle.Difficulty.Skills.Physical;
using UniCircleTools.Beatmaps;

namespace UniCircle.Difficulty.Standard.Skills.Physical.Clicking
{
    /// <summary>
    /// Skill representing the difficulty of keeping up with tapping speed of notes
    /// </summary>
    public class Clicking : PhysicalSkill<ClickPoint>
    {
        // Shortcuts for readability
        private ClickPoint ClickPointA => GetDifficultyPoint(0);
        private ClickPoint ClickPointB => GetDifficultyPoint(1);

        // Exertion decay rate
        public override double SpeedDecayBaseMin { get; set; } = 0.1;
        public override double StaminaDecayBaseMin { get; set; } = 0.9;
        public override double ExertionDecayNormaliser { get; set; } = 150;

        // Exertion weights
        public override double SpeedWeight { get; set; } = 1;
        public override double StaminaWeight { get; set; } = 1;

        public override double SkillMultiplier { get; set; } = 10;

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

        // Energy exerted in a key press can be taken as a constant since there is no varying pressure or anything
        protected override double CalculateEnergyExerted()
        {
            // In ppv2, higher spaced objects are worth more to reward spaced streams.
            // This can is really part of aim, and thus speed is not concerned with it.
            return 1;
        }

        protected override double CalculateSemanticBonus()
        {
            // Accuracy difficulty assuming perfect reading (essentially just changes in beat snapping)
            return 0;
        }
    }
}
