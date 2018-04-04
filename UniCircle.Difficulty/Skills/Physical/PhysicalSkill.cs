using System;

namespace UniCircle.Difficulty.Skills.Physical
{
    public abstract class PhysicalSkill<TDiffPoint> : Skill<TDiffPoint> where TDiffPoint : PhysicalPoint
    {
        // Exertion values
        private double _speed;
        private double _stamina;

        /// <summary>
        /// Minimum fraction <see cref="_speed"/> can decay to in 1 second
        /// </summary>
        public abstract double SpeedDecayBaseMin { get; set; }
        
        /// <summary>
        /// Minimum fraction <see cref="_stamina"/> can decay to in 1 second
        /// </summary>
        public abstract double StaminaDecayBaseMin { get; set; }

        /// <summary>
        /// Value that normalises the decay function to have a reasonable value range
        /// </summary>
        public abstract double ExertionDecayNormaliser { get; set; }

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
            // Calculate difficulty and exertion values
            double energyExerted = CalculateEnergyExerted();
            double semanticBonus = CalculateSemanticBonus();

            // Add to exertion values
            _speed += energyExerted;
            _stamina += energyExerted;

            // Decay exertion values (note this is done after exertion values are added so the current action is included in the decay)
            _speed *= SpeedDecay(diffPoint.DeltaTime, energyExerted);
            _stamina *= StaminaDecay(diffPoint.DeltaTime, energyExerted);

            // Perhaps square deltatime since exertion doesnt include time anymore.
            double rawDifficulty = energyExerted / diffPoint.DeltaTime;
            double speedBonus = _speed * SpeedWeight;
            double staminaBonus = _stamina * StaminaWeight;

            diffPoint.Difficulty = rawDifficulty * (1 + semanticBonus + speedBonus + staminaBonus);

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

        private double SpeedDecay(double time, double energy) => Math.Pow(-(1 - SpeedDecayBaseMin) * Math.Pow(Math.E, -ExertionDecayNormaliser * energy / time) + 1, time / 1000);
        private double StaminaDecay(double time, double energy) => Math.Pow(-(1 - StaminaDecayBaseMin) * Math.Pow(Math.E, -ExertionDecayNormaliser * energy / time) + 1, time / 1000);

        /// <inheritdoc />
        public override void Reset()
        {
            _speed = 0;
            _stamina = 0;

            base.Reset();
        }
    }
}
