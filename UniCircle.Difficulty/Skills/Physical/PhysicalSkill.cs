using System;

namespace UniCircle.Difficulty.Skills.Physical
{
    public abstract class PhysicalSkill<TDiffPoint> : Skill<TDiffPoint> where TDiffPoint : PhysicalPoint
    {
        // Fraction excertion values decay to in 1 second
        public abstract double SpeedDecayBase { get; set; }
        public abstract double StaminaDecayBase { get; set; }

        // Weight of exertion values
        public abstract double SpeedWeight { get; set; }
        public abstract double StaminaWeight { get; set; }

        // Exertion values
        private double _speed;
        private double _stamina;

        protected override void CalculateDifficulty()
        {
            TDiffPoint diffPoint = GetDifficultyPoint(0);
            // Calculate difficulty and exertion values
            double energyExerted = CalculateEnergyExerted();
            double semanticBonus = CalculateSemanticBonus();

            // Add to exertion values
            _speed += energyExerted;
            _stamina += energyExerted;

            // Decay exertion values (note this is done after exertion values are added so the current action is included in the decay)
            _speed *= SpeedDecay(diffPoint.DeltaTime);
            _stamina *= StaminaDecay(diffPoint.DeltaTime);

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
        /// Calculates the total energy exerted in the action described by the current difficulty point.
        /// This value is added to <see cref="_exertion"/>.
        /// This value is should be independant of time.
        /// </summary>
        /// <returns>Total energy exerted during current point</returns>
        protected abstract double CalculateEnergyExerted();

        /// <summary>
        /// Calculates the bonus difficulty of an object based on semantics
        /// </summary>
        /// <returns>Semantic difficulty multiplier of the current object</returns>
        protected virtual double CalculateSemanticBonus() => 0;

        private double SpeedDecay(double time) => Math.Pow(SpeedDecayBase, time / 1000);
        private double StaminaDecay(double time) => Math.Pow(StaminaDecayBase, time / 1000);

        public override void Reset()
        {
            _speed = 0;
            _stamina = 0;

            base.Reset();
        }
    }
}
