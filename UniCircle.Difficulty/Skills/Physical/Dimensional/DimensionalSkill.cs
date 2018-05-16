using System;

namespace UniCircle.Difficulty.Skills.Physical.Dimensional
{
    public abstract class DimensionalSkill<TDiffPoint> : PhysicalSkill<TDiffPoint>
        where TDiffPoint : DimensionalPoint
    {
        private Vector previousForce;

        // Shortcut for readability
        private DimensionalPoint DimensionalPointA => GetDifficultyPoint(0);

        protected override double CalculateEnergyExerted()
        {
            // Calculate snappiness
            double snappiness = CalculateSnappiness();

            // Calculate snap and flow energy
            double snapEnergy = CalculateSnappingEnergy();
            double flowEnergy = CalculateFlowingEnergy();

            // Sum up parts with weights
            return snapEnergy * snappiness + flowEnergy * (1 - snappiness);
        }

        private double CalculateSnappiness()
        {
            // TODO: Implement snapiness (decaying force flux) value between 0 and 1
            return 1;
        }

        private double CalculateSnappingEnergy()
        {
            // Moving and stopping force are both proportional to the vector length
            //  ie. the larger the jump, the higher the amount of force to get there and thus higher force to cancel it out
            // TODO: contemplate if moving and stopping power are equal and perhaps need sliding window variable for skills to adjust
            Vector movingForce = DimensionalPointA.IncomingForce;
            Vector stoppingForce = -DimensionalPointA.IncomingForce;

            // vector opposing the direction of movement
            previousForce = stoppingForce;

            return movingForce.Length + stoppingForce.Length;
        }

        private double CalculateFlowingEnergy()
        {
            // Just distance right?
            Vector movingForce = DimensionalPointA.IncomingForce;

            // vector in the direction of movement
            previousForce = movingForce;

            return movingForce.Length;
        }
    }
}
