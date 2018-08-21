using System;
using UniCircleTools.Beatmaps;

namespace UniCircle.Difficulty.Skills.Physical.Dimensional
{
    public abstract class DimensionalSkill : PhysicalSkill
    {
        private Vector _previousSnapForce;
        private Vector _previousActualForce;
        private double _snapForceVolatility;
        private double _actualForceVolatility;

        public abstract double SnapForceThreshold { get; set; }
        public abstract double FlowForceThreshold { get; set; }

        public abstract double SnapForceVolatilityRecoveryRate { get; set; }

        // TODO: refactor this. i dont like it
        private HitObject _previousHitObject;
        private double _deltaTime;

        public override void ProcessHitObject(HitObject hitObject)
        {
            if (_previousHitObject != null)
            {
                _deltaTime = hitObject.Time - _previousHitObject.Time;
            }
            else
            {
                _deltaTime = 0;
            }

            _previousHitObject = hitObject;

            base.ProcessHitObject(hitObject);
        }

        protected override double CalculateEnergyExerted()
        {
            // Recover
            _snapForceVolatility *= 1 - ForceVolatilityRecovery(_deltaTime);
            _actualForceVolatility *= 1 - ForceVolatilityRecovery(_deltaTime);

            // Calculate snappiness
            double snappiness = CalculateSnappiness();

            // Calculate snap and flow energy
            double snapEnergy = CalculateSnappingEnergy(snappiness);    // for now, snap needs to be done first, because flowing uses _previousSnapForce, which is set in snapping
            double flowEnergy = CalculateFlowingEnergy(snappiness);     // TODO: refactor to fix that ^

            // Data points
            DataPoints.Add("Snap Force Volatility", _snapForceVolatility);
            DataPoints.Add("Actual Force Volatility", _actualForceVolatility);
            DataPoints.Add("Snappiness", snappiness);

            // Sum up parts with weights
            return snapEnergy + flowEnergy;
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

        private double CalculateSnappingEnergy(double snappiness)
        {
            // Moving and stopping force are both proportional to the vector length
            //  ie. the larger the jump, the higher the amount of force to get there and thus higher force to cancel it out

            Vector movingForce = CalculateIncomingForce();
            Vector stoppingForce = -CalculateIncomingForce();

            // Add force flux values
            if (_previousSnapForce != null)
            {
                _snapForceVolatility += (movingForce.UnitVector - _previousSnapForce.UnitVector).Length;
                _actualForceVolatility += (movingForce.UnitVector - _previousActualForce.UnitVector).Length * snappiness;
            }
            _snapForceVolatility += (stoppingForce.UnitVector - movingForce.UnitVector).Length;
            _snapForceVolatility += (stoppingForce.UnitVector - movingForce.UnitVector).Length * snappiness;

            // vector opposing the direction of movement
            _previousSnapForce = stoppingForce;

            return (movingForce.Length + stoppingForce.Length) * snappiness;
        }

        private double CalculateFlowingEnergy(double snappiness)
        {
            // Just distance right?
            Vector movingForce = CalculateIncomingForce();

            // Add force flux values
            if (_previousActualForce != null)
            {
                _actualForceVolatility += (movingForce.UnitVector - _previousActualForce.UnitVector).Length * (1 - snappiness);
            }

            // Update previous force (weighted vector of snap and flow)
            _previousActualForce = _previousSnapForce * snappiness + movingForce * (1 - snappiness);

            return movingForce.Length * (1 - snappiness);
        }

        private double ForceVolatilityRecovery(double time) => 1 - Math.Pow(1 - SnapForceVolatilityRecoveryRate, time / 1000);
        
        protected override double CalculateImprecision()
        {
            double distance = CalculateIncomingForce().Length;
            double targetRange = CalculateTargetErrorRange();
            double snappiness = CalculateSnappiness();

            if (distance < targetRange)
            {
                // target range touches the expected current cursor position so from the moment the next action begins you are already in the range
                return _deltaTime;
            }

            double targetPortion = targetRange / distance;  // circle radius to distance ratio
            double targetThreshold = 1 - targetPortion;     // portion of distance to reach target edge
            
            // temporary simple curve until more research is put into snap speeds
            double timeToThresholdPortion = snappiness * (Math.Acos(-2 * targetThreshold + 1) / Math.PI) + (1 - snappiness) * targetThreshold;  // time to reach edge of target circle
            double targetTimePortion = 1 - timeToThresholdPortion;  // time within target circle

            return _deltaTime * targetTimePortion;
        }

        protected abstract Vector CalculateIncomingForce();

        protected abstract double CalculateTargetErrorRange();

        /// <inheritdoc />
        public override void Reset()
        {
            _snapForceVolatility = 0;
            _actualForceVolatility = 0;
            _previousSnapForce = null;
            _previousActualForce = null;

            base.Reset();
        }
    }
}
