using UniCircleTools.Beatmaps;

using UniCircle.Difficulty.Skills.Physical.Dimensional;

namespace UniCircle.Difficulty.Standard.Skills.Physical.Aiming
{
    /// <summary>
    /// Skill representing the difficulty of moving your cursor between notes
    /// </summary>
    public class Aiming : DimensionalSkill
    {
        // Exertion recovery rate
        public override double MaxSpeedRecoveryRate { get; set; } = 0.9;
        public override double MaxStaminaRecoveryRate { get; set; } = 0.1;
        public override double ExertionNormaliser { get; set; } = 1;

        // Exertion weights
        public override double SpeedWeight { get; set; } = 0.0005;
        public override double StaminaWeight { get; set; } = 0.00005;

        public override double SnapForceThreshold { get; set; } = 5;
        public override double FlowForceThreshold { get; set; } = 13;
        public override double SnapForceVolatilityRecoveryRate { get; set; } = 0.9;

        private Vector _previousPosition;

        private Vector _currentPosition;
        private HitObject _currentHitObject;

        public override bool ProcessHitObject(HitObject hitObject)
        {
            if (hitObject is Spinner)
            {
                // Spinners are not considered in aim
                return false;
            }
            
            _previousPosition = _currentPosition;

            _currentHitObject = hitObject;
            _currentPosition = new Vector(hitObject.X, hitObject.Y);

            return base.ProcessHitObject(hitObject);
        }

        protected override Vector CalculateIncomingForce() => _currentPosition - (_previousPosition ?? _currentPosition);

        protected override double CalculateTargetErrorRange() => _currentHitObject.Radius;
    }
}
