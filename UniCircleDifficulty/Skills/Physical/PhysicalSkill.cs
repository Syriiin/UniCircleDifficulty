using System;
using System.Collections.Generic;
using System.Text;

using UniCircleTools;

namespace UniCircleDifficulty.Skills.Physical
{
    abstract class PhysicalSkill<TDiffPoint> : Skill<TDiffPoint> where TDiffPoint : DifficultyPoint
    {
        // Fraction excertion values decay to in 1 second
        protected abstract double SpeedDecayBase { get; }
        protected abstract double StaminaDecayBase { get; }

        // Exertion values
        private double _speed = 1;
        private double _stamina = 1;

        protected override void CalculateDifficulty()
        {
            DifficultyPoint diffPoint = GetDifficultyPoint(0);
            // Calculate difficulty and exertion values
            double energyExerted = CalculateEnergyExerted();
            double bonusMultiplier = CalculateBonusMultiplier();
            // Experimental deltatime squaring since exertion doesnt include time anymore.
            // Also probably want to apply a transformation to exertion values?
            diffPoint.Difficulty = energyExerted * bonusMultiplier * _speed * _stamina / Math.Pow(diffPoint.DeltaTime, 2);

            // Add to exertion values
            _speed += energyExerted;
            //_stamina += energyExerted;

            // Decay exertion values
            _speed *= SpeedDecay(diffPoint.DeltaTime);
            //_stamina *= StaminaDecay(diffPoint.DeltaTime);
        }

        /// <summary>
        /// Calculates the total energy exerted in the action described by the current difficulty point.
        /// This value is added to <see cref="_exertion"/>.
        /// This value is should be independant of time.
        /// </summary>
        /// <returns>Total energy exerted during current point</returns>
        protected abstract double CalculateEnergyExerted();

        /// <summary>
        /// Calculates the bonus difficulty multiplier which affects the value added to <see cref="_diffList"/>.
        /// This value accounts for semantic difficulty beyond
        /// </summary>
        /// <returns>Bonus difficulty multiplier of the current object</returns>
        protected virtual double CalculateBonusMultiplier() => 1;

        private double SpeedDecay(double time) => Math.Pow(SpeedDecayBase, time / 1000);
        private double StaminaDecay(double time) => Math.Pow(StaminaDecayBase, time / 1000);

        public PhysicalSkill(Mods mods) : base(mods) { }
    }
}
