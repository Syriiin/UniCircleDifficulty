using System;
using System.Collections.Generic;
using System.Linq;

using UniCircleTools;
using UniCircleTools.Beatmaps;

namespace UniCircle.Difficulty.Skills
{
    /// <summary>
    /// Represents a specific skill that adds difficulty to a beatmap
    /// </summary>
    public abstract class Skill<TDiffPoint> : ISkill where TDiffPoint : DifficultyPoint
    {
        private const double DiffWeight = 0.9;
        
        /// <summary>
        /// Mods to consider in hitobject processing
        /// </summary>
        protected Mods Mods;

        /// <summary>
        /// List of <see cref="DifficultyPoint"/>s this skill uses to calculate difficulty for the latest point
        /// </summary>
        protected List<TDiffPoint> CurrentDiffPoints = new List<TDiffPoint>();

        /// <summary>
        /// List of calculated difficulty points
        /// </summary>
        public List<TDiffPoint> CalculatedPoints { get; } = new List<TDiffPoint>();

        /// <summary>
        /// Multiplier to scale difficulty rating to a consistent value range across skills
        /// </summary>
        public abstract double SkillMultiplier { get; set; }

        /// <summary>
        /// Current total difficulty value based on <see cref="CalculatedPoints"/>
        /// </summary>
        public double Value
        {
            get
            {
                var diffPoints = CalculatedPoints.OrderByDescending(d => d.Difficulty).ToList();
                double total = 0;
                double i = 0;

                foreach (var diffPoint in diffPoints)
                {
                    total += diffPoint.Difficulty * Math.Pow(DiffWeight, i);
                    i += diffPoint.DeltaTime / 400;
                }

                // Apply difficulty curve and normalise with multiplier
                return Math.Sqrt(total) * SkillMultiplier;
            }
        }

        /// <summary>
        /// Shortcut method for accessing <see cref="CurrentDiffPoints"/> with reverse indexing
        /// </summary>
        /// <param name="objectNum">Reverse index of object to return</param>
        protected TDiffPoint GetDifficultyPoint(int objectNum) => CurrentDiffPoints.ElementAtOrDefault(CurrentDiffPoints.Count - (1 + objectNum));

        /// <summary>
        /// Process multiple <see cref="HitObject"/>s in the order provided by the passed <see cref="IEnumerable{HitObject}"/>
        /// </summary>
        /// <param name="hitObjects">Collection of <see cref="HitObject"/>s to process</param>
        public void ProcessHitObjectSequence(IEnumerable<HitObject> hitObjects)
        {
            foreach (var hitObject in hitObjects)
            {
                ProcessHitObject(hitObject);
            }
        }

        /// <summary>
        /// Process <see cref="HitObject"/> by converting it into <see cref="DifficultyPoint"/>s and calculating difficulty
        /// </summary>
        /// <param name="hitObject">HitObject to process</param>
        public abstract void ProcessHitObject(HitObject hitObject);

        /// <summary>
        /// Processes <see cref="DifficultyPoint"/> and calculates difficulty
        /// </summary>
        /// <param name="diffPoint"></param>
        protected void ProcessDifficultyPoint(TDiffPoint diffPoint)
        {
            // Update diffpoint pool
            UpdateDifficultyPoints(diffPoint);

            // Calculate difficulty of point and add to list
            CalculateDifficulty();
            CalculatedPoints.Add(diffPoint);
        }

        /// <summary>
        /// Add <see cref="DifficultyPoint"/> to <see cref="CurrentDiffPoints"/> and remove any now unneeded points
        /// </summary>
        /// <param name="diffPoint"><see cref="DifficultyPoint"/> to add</param>
        protected abstract void UpdateDifficultyPoints(TDiffPoint diffPoint);

        /// <summary>
        /// Calculates and sets difficulty of the current latest <see cref="DifficultyPoint"/> with <see cref="CurrentDiffPoints"/> as context
        /// </summary>
        protected abstract void CalculateDifficulty();

        /// <summary>
        /// Resets skill to default state
        /// </summary>
        public virtual void Reset()
        {
            CurrentDiffPoints.Clear();
            CalculatedPoints.Clear();
        }

        /// <summary>
        /// Sets <see cref="Mods"/>
        /// </summary>
        /// <param name="mods"><see cref="UniCircleTools.Mods"/> to set</param>
        public void SetMods(Mods mods)
        {
            Mods = mods;
        }
    }
}
