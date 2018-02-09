using System;
using System.Collections.Generic;
using System.Linq;

using UniCircleTools;
using UniCircleTools.Beatmaps;

namespace UniCircleDifficulty.Skills
{
    /// <summary>
    /// Represents a specific skill that adds difficulty to a beatmap
    /// </summary>
    public abstract class Skill<TDiffPoint> : ISkill where TDiffPoint : DifficultyPoint
    {
        /// <summary>
        /// Base weight for calculating difficulty totals
        /// </summary>
        private const double diff_weight = 0.9;
        
        /// <summary>
        /// Mods to consider in hitobject processing
        /// </summary>
        protected Mods _mods;

        /// <summary>
        /// List of <see cref="DifficultyPoint"/>s this skill needs to calculate difficulty for the latest point
        /// </summary>
        protected List<TDiffPoint> _currentDiffPoints = new List<TDiffPoint>();

        /// <summary>
        /// Shortcut method for accessing <see cref="_currentDiffPoints"/> with reverse indexing
        /// </summary>
        /// <param name="objectNum">Reverse index of object to return</param>
        /// <returns></returns>
        protected TDiffPoint GetDifficultyPoint(int objectNum) => _currentDiffPoints.ElementAtOrDefault(_currentDiffPoints.Count - (1 + objectNum));

        /// <summary>
        /// List of calculated difficulty points
        /// </summary>
        public List<TDiffPoint> CalculatedPoints { get; } = new List<TDiffPoint>();

        /// <summary>
        /// Multiplier to scale difficulty rating to a consistent value range across skills
        /// </summary>
        protected abstract double SkillMultiplier { get; }

        /// <summary>
        /// Current total difficulty value based on <see cref="_diffList"/>
        /// </summary>
        public double Value
        {
            get
            {
                // Perhaps instead each should be weighted against the max value
                // ie. values closer to the max value should contribute more, meaning 5m 1* + 1m 3* makes a 3* map, but 5m 3* makes a 3.5* or something
                List<TDiffPoint> diffPoints = CalculatedPoints.OrderByDescending(d => d.Difficulty).ToList();
                double total = 0;
                double i = 0;

                foreach (TDiffPoint diffPoint in diffPoints)
                {
                    total += diffPoint.Difficulty * Math.Pow(diff_weight, i);
                    i += diffPoint.DeltaTime / 400;
                }

                // Apply difficulty curve and normalise with multiplier
                return Math.Sqrt(total) * SkillMultiplier;
            }
        }

        /// <summary>
        /// Process multiple <see cref="HitObject"/>s in order provided by the passed <see cref="IEnumerable{HitObject}"/>
        /// </summary>
        /// <param name="hitObjects">Collection of HitObjects to process</param>
        public void ProcessHitObjectSequence(IEnumerable<HitObject> hitObjects)
        {
            foreach (HitObject hitObject in hitObjects)
            {
                ProcessHitObject(hitObject);
            }
        }

        /// <summary>
        /// Converts <see cref="HitObject"/>s into <see cref="DifficultyPoint"/>s and calls <see cref="ProcessDifficultyPoint(TDiffPoint)"/> with them
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
        /// Add <see cref="DifficultyPoint"/> to <see cref="_currentDiffPoints"/> and remove any now irrelevent points
        /// </summary>
        /// <param name="diffPoint"><see cref="DifficultyPoint"/> to add</param>
        protected abstract void UpdateDifficultyPoints(TDiffPoint diffPoint);

        /// <summary>
        /// Calculates and sets difficulty of the current latest diff point with the rest as context
        /// </summary>
        protected abstract void CalculateDifficulty();

        public virtual void Reset()
        {
            _currentDiffPoints.Clear();
            CalculatedPoints.Clear();
        }

        public void SetMods(Mods mods)
        {
            _mods = mods;
        }
    }
}
