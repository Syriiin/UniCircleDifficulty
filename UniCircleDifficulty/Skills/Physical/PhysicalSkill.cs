using System;
using System.Collections.Generic;
using System.Text;

using UniCircleTools;

namespace UniCircleDifficulty.Skills.Physical
{
    abstract class PhysicalSkill<TDiffPoint> : Skill<TDiffPoint> where TDiffPoint : PhysicalPoint
    {
        // Fraction excertion values decay to in 1 second
        protected abstract double SpeedDecayBase { get; }
        protected abstract double StaminaDecayBase { get; }

        // Weight of exertion values
        protected abstract double SpeedWeight { get; }
        protected abstract double StaminaWeight { get; }

        // Exertion values
        private double _speed = 0;
        private double _stamina = 0;

        protected override void CalculateDifficulty()
        {
            TDiffPoint diffPoint = GetDifficultyPoint(0);
            // Calculate difficulty and exertion values
            double energyExerted = CalculateEnergyExerted();
            double semanticBonus = CalculateSemanticBonus();
            // Experimental deltatime squaring since exertion doesnt include time anymore.
            double rawDifficulty = energyExerted / Math.Pow(diffPoint.DeltaTime, 2);
            double speedBonus = _speed * SpeedWeight;
            double staminaBonus = _stamina * StaminaWeight;

            diffPoint.Difficulty = rawDifficulty * (semanticBonus + speedBonus + staminaBonus);

            // Data points
            diffPoint.CurrentSpeed = _speed;
            diffPoint.CurrentStamina = _stamina;

            // Add to exertion values
            _speed += energyExerted;
            _stamina += energyExerted;

            // Decay exertion values (note this is done after exertion values are added so the current action is included in the decay)
            _speed *= SpeedDecay(diffPoint.DeltaTime);
            _stamina *= StaminaDecay(diffPoint.DeltaTime);
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
        protected virtual double CalculateSemanticBonus() => 1;

        private double SpeedDecay(double time) => Math.Pow(SpeedDecayBase, time / 1000);
        private double StaminaDecay(double time) => Math.Pow(StaminaDecayBase, time / 1000);

        public PhysicalSkill(Mods mods) : base(mods) { }
    }
}
