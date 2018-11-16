using System;
using System.Collections.Generic;

using UniCircleTools.Beatmaps;

namespace UniCircle.Difficulty.Skills.Physical
{
    /// <summary>
    /// Abstract class to assist when creating a skill centered around physical exertion
    /// </summary>
    public abstract class PhysicalSkill : ISkill
    {
        // Exertion values
        private double _speed;
        private double _stamina;
        
        /// <summary>
        /// Maximum speed percent recoverable in 1 second
        /// </summary>
        public abstract double MaxSpeedRecoveryRate { get; set; }

        /// <summary>
        /// Maximum stamina percent recoverable in 1 second
        /// </summary>
        public abstract double MaxStaminaRecoveryRate { get; set; }

        /// <summary>
        /// Value that normalises the decay function to have a reasonable value range
        /// </summary>
        public abstract double ExertionNormaliser { get; set; }

        /// <summary>
        /// Weight of <see cref="_speed"/> for difficulty
        /// </summary>
        public abstract double SpeedWeight { get; set; }

        /// <summary>
        /// Weight of <see cref="_stamina"/> for difficulty
        /// </summary>
        public abstract double StaminaWeight { get; set; }

        /// <summary>
        /// Calculates the total energy exerted (independant of time) in the action described by the latest <see cref="HitObject"/>
        /// </summary>
        /// <returns>Total energy exerted during current point</returns>
        protected abstract double CalculateEnergyExerted();

        /// <summary>
        /// Calculates the level of accuracy required by the action described by the latest <see cref="HitObject"/>
        /// </summary>
        /// <returns>Calculated imprecision</returns>
        protected abstract double CalculateImprecision();

        /// <summary>
        /// Calculates any bonus difficulty of a <see cref="HitObject"/> based on semantics
        /// </summary>
        /// <returns>Semantic difficulty multiplier of the current object</returns>
        protected virtual double CalculateSemanticBonus() => 0;
        
        // Dynamic recovery rate such that less exertion is recovered while performing high exertion actions
        private double ExertionRecoveryRate(double maxRecoveryRate, double rateOfExertion) => maxRecoveryRate * Math.Pow(Math.E, -rateOfExertion * ExertionNormaliser);
        private double SpeedRecovery(double time, double energy) => 1 - Math.Pow(1 - ExertionRecoveryRate(MaxSpeedRecoveryRate, energy / time), time / 1000);
        private double StaminaRecovery(double time, double energy) => 1 - Math.Pow(1 - ExertionRecoveryRate(MaxStaminaRecoveryRate, energy / time), time / 1000);

        // TODO: refactor this. i dont like it
        private HitObject _previousHitObject;
        private double _deltaTime;

        /// <inheritdoc />
        public virtual bool ProcessHitObject(HitObject hitObject)
        {
            _deltaTime = hitObject.Time - _previousHitObject?.Time ?? hitObject.Time;

            _previousHitObject = hitObject;

            return true;
        }

        /// <inheritdoc />
        public double CalculateDifficulty()
        {
            // Calculate energy, semantic, and error range values
            double energyExerted = CalculateEnergyExerted();
            double semanticBonus = CalculateSemanticBonus();
            double imprecision = CalculateImprecision();

            // Add to exertion values
            _speed += energyExerted;
            _stamina += energyExerted;

            // Recover exertion values (note this is done after exertion values are added so the current action is included in the recovery) (this needs to be revisited)
            _speed *= 1 - SpeedRecovery(_deltaTime, energyExerted);
            _stamina *= 1 - StaminaRecovery(_deltaTime, energyExerted);

            // Imprecision for binary skills (clicking) is the hit window,
            //  and for dimensional skills, is the time spent within the error range when moving in the expected path according to flow and snap motions
            double rawDifficulty = 1 / imprecision;
            double speedBonus = _speed * SpeedWeight;
            double staminaBonus = _stamina * StaminaWeight;

            // Data points
            DataPoints.Add("Current Speed", _speed);
            DataPoints.Add("Current Stamina", _stamina);
            DataPoints.Add("Imprecision", imprecision);

            return rawDifficulty * (1 + semanticBonus) * (1 + speedBonus) * (1 + staminaBonus);
        }
        
        /// <inheritdoc />
        public Dictionary<string, double> DataPoints { get; set; }

        /// <inheritdoc />
        public virtual void Reset()
        {
            _speed = 0;
            _stamina = 0;
            _deltaTime = 0;
            _previousHitObject = null;
        }
    }
}
