using UniCircleTools.Beatmaps;

using UniCircle.Difficulty.Skills.Physical.Binary;

namespace UniCircle.Difficulty.Standard.Skills.Physical.Clicking
{
    /// <summary>
    /// Skill representing the difficulty of keeping up with tapping speed of notes
    /// </summary>
    public class Clicking : BinarySkill
    {
        // Exertion recovery rate
        public override double MaxSpeedRecoveryRate { get; set; } = 0.9;
        public override double MaxStaminaRecoveryRate { get; set; } = 0.1;
        public override double ExertionNormaliser { get; set; } = 150;

        // Exertion weights
        public override double SpeedWeight { get; set; } = 0.5;
        public override double StaminaWeight { get; set; } = 0.05;

        private HitObject _currentHitObject;

        public override bool ProcessHitObject(HitObject hitObject)
        {
            if (hitObject is Spinner)
            {
                // Spinners are not considered in clicking
                return false;
            }

            _currentHitObject = hitObject;

            return base.ProcessHitObject(hitObject);
        }

        protected override double CalculateImprecision() => _currentHitObject.HitWindowFor(HitResult.Hit300);
    }
}
