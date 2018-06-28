using System;

namespace UniCircle.Difficulty.Skills.Physical
{
    public abstract class PhysicalSkill<TDiffPoint> : Skill<TDiffPoint> where TDiffPoint : PhysicalPoint
    {
        // Exertion values
        private double _speed;
        private double _stamina;
        
        /// <summary>
        /// Maximum speed recoverable in 1 second
        /// </summary>
        public abstract double MaxSpeedRecoveryRate { get; set; }

        /// <summary>
        /// Maximum stamina recoverable in 1 second
        /// </summary>
        public abstract double MaxStaminaRecoveryRate { get; set; }

        /// <summary>
        /// Value that normalises the decay function to have a reasonable value range
        /// </summary>
        public abstract double ExertionNormaliser { get; set; }

        // Weight of exertion values
        /// <summary>
        /// Weight of <see cref="_speed"/>
        /// </summary>
        public abstract double SpeedWeight { get; set; }

        /// <summary>
        /// Weight of <see cref="_stamina"/>
        /// </summary>
        public abstract double StaminaWeight { get; set; }

        /// <inheritdoc />
        protected override void CalculateDifficulty()
        {
            var diffPoint = GetDifficultyPoint(0);
            // Calculate energy, semantic, and error range values
            double energyExerted = CalculateEnergyExerted();
            double semanticBonus = CalculateSemanticBonus();

            // Add to exertion values
            _speed += energyExerted;
            _stamina += energyExerted;

            // Recover exertion values (note this is done after exertion values are added so the current action is included in the recovery) (this needs to be revisited)
            _speed *= 1 - SpeedRecovery(diffPoint.DeltaTime, energyExerted);
            _stamina *= 1 - StaminaRecovery(diffPoint.DeltaTime, energyExerted);
            
            // Imprecision for binary skills (clicking) is the hit window,
            //  and for dimensional skills, is the time spent within the error range when moving in the expected path according to flow and snap motions
            double rawDifficulty = 1 / diffPoint.Imprecision;
            double speedBonus = _speed * SpeedWeight;
            double staminaBonus = _stamina * StaminaWeight;

            diffPoint.Difficulty = rawDifficulty * (1 + semanticBonus) * (1 + speedBonus) * (1 + staminaBonus);

            // Data points
            diffPoint.CurrentSpeed = _speed;
            diffPoint.CurrentStamina = _stamina;
        }

        /// <summary>
        /// Calculates the total energy exerted (independant of time) in the action described by the latest difficulty point
        /// </summary>
        /// <returns>Total energy exerted during current point</returns>
        protected abstract double CalculateEnergyExerted();

        /// <summary>
        /// Calculates the bonus difficulty of a difficulty point based on semantics
        /// </summary>
        /// <returns>Semantic difficulty multiplier of the current object</returns>
        protected virtual double CalculateSemanticBonus() => 0;
        
        // Dynamic recovery rate such that less exertion is recovered while performing high exertion actions
        private double ExertionRecoveryRate(double maxRecoveryRate, double rateOfExertion) => maxRecoveryRate * Math.Pow(Math.E, -rateOfExertion * ExertionNormaliser);
        private double SpeedRecovery(double time, double energy) => 1 - Math.Pow(1 - ExertionRecoveryRate(MaxSpeedRecoveryRate, energy / time), time / 1000);
        private double StaminaRecovery(double time, double energy) => 1 - Math.Pow(1 - ExertionRecoveryRate(MaxStaminaRecoveryRate, energy / time), time / 1000);

        /// <inheritdoc />
        public override void Reset()
        {
            _speed = 0;
            _stamina = 0;

            base.Reset();
        }
    }
}
