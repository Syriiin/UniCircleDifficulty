using System;

namespace UniCircle.Difficulty.Skills.Physical.Dimensional
{
    public abstract class DimensionalSkill<TDiffPoint> : PhysicalSkill<TDiffPoint>
        where TDiffPoint : DimensionalPoint
    {
        private Vector _previousSnapForce;
        private double _snapForceVolatility;

        // Shortcut for readability
        private DimensionalPoint DimensionalPointA => GetDifficultyPoint(0);

        public abstract double SnapForceThreshold { get; set; }
        public abstract double FlowForceThreshold { get; set; }

        public abstract double SnapForceVolatilityRecoveryRate { get; set; }

        protected override double CalculateEnergyExerted()
        {
            // Recover
            _snapForceVolatility *= 1 - SnapForceVolatilityRecovery(DimensionalPointA.DeltaTime);

            // Calculate snap and flow energy
            double snapEnergy = CalculateSnappingEnergy();
            double flowEnergy = CalculateFlowingEnergy();

            // Calculate snappiness
            double snappiness = CalculateSnappiness();

            // Data points
            DimensionalPointA.SnapForceVolatility = _snapForceVolatility;
            DimensionalPointA.Snappiness = CalculateSnappiness();

            // Sum up parts with weights
            return snapEnergy * snappiness + flowEnergy * (1 - snappiness);
        }

        private double CalculateSnappiness()
        {
            double snappiness;

            if (_snapForceVolatility < SnapForceThreshold)
            {
                snappiness = 1;
            }
            else if (_snapForceVolatility > FlowForceThreshold)
            {
                snappiness = 0;
            }
            else
            {
                snappiness = (Math.Cos(Math.PI * (_snapForceVolatility - SnapForceThreshold) / (SnapForceThreshold - FlowForceThreshold)) + 1) / 2;
            }

            return snappiness;
        }

        private double CalculateSnappingEnergy()
        {
            // Moving and stopping force are both proportional to the vector length
            //  ie. the larger the jump, the higher the amount of force to get there and thus higher force to cancel it out

            // TODO: contemplate if moving and stopping power are equal and perhaps need sliding window variable for skills to adjust

            Vector movingForce = DimensionalPointA.IncomingForce;
            Vector stoppingForce = -DimensionalPointA.IncomingForce;

            // Add snap force flux values
            if (_previousSnapForce != null)
            {
                _snapForceVolatility += (movingForce.UnitVector - _previousSnapForce.UnitVector).Length;
            }
            _snapForceVolatility += (stoppingForce.UnitVector - movingForce.UnitVector).Length;


            // vector opposing the direction of movement
            _previousSnapForce = stoppingForce;

            return movingForce.Length + stoppingForce.Length;
        }

        private double CalculateFlowingEnergy()
        {
            // Just distance right?
            Vector movingForce = DimensionalPointA.IncomingForce;

            return movingForce.Length;
        }

        private double SnapForceVolatilityRecovery(double time) => 1 - Math.Pow(1 - SnapForceVolatilityRecoveryRate, time / 1000);

        /// <inheritdoc />
        public override void Reset()
        {
            _snapForceVolatility = 0;
            _previousSnapForce = null;

            base.Reset();
        }
    }
}
