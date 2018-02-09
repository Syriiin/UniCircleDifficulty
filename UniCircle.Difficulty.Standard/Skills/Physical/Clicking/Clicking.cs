using System;

using UniCircleDifficulty.Skills.Physical;
using UniCircleTools;
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
        protected override double SpeedDecayBase => 0.3;
        protected override double StaminaDecayBase => 0.7;

        // Exertion weights
        protected override double SpeedWeight => 1;
        protected override double StaminaWeight => 1;

        protected override double SkillMultiplier => 10;

        public override void ProcessHitObject(HitObject hitObject)
        {
            if (hitObject is Spinner)
            {
                // Spinners are not considered in speed
                return;
            }

            double offset = hitObject.Time / Utils.ModClockRate(_mods);

            // Construct click point from hitobject and call ProcessDifficultyPoint with them
            ClickPoint clickPoint = new ClickPoint
            {
                DeltaTime = offset - ClickPointB?.Offset ?? offset,
                Offset = offset
            };

            ProcessDifficultyPoint(clickPoint);
        }

        protected override void UpdateDifficultyPoints(ClickPoint clickPoint)
        {
            // Add diffPoint to currentDiffPoints
            _currentDiffPoints.Add(clickPoint);

            // Update pool
            if (_currentDiffPoints.Count == 3)
            {
                _currentDiffPoints.RemoveAt(0);
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
            return base.CalculateSemanticBonus();
        }
    }
}
