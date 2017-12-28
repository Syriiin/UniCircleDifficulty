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
    abstract class Skill<TDiffPoint> where TDiffPoint : DifficultyPoint
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
        /// Value that represents the current difficulty, including lingering difficulty. 
        /// This value is taken as the raw difficulty at a given point. Similar to strain in ppv2
        /// </summary>
        private double _excertion = 1;

        /// <summary>
        /// Fraction excertion decays to in 1 second
        /// </summary>
        protected abstract double ExcertionDecayBase { get; }

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
        /// List of difficulty values after processing each object
        /// </summary>
        private List<double> _diffList = new List<double>();

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
                List<double> diffValues = _diffList.OrderByDescending(d => d).ToList();
                double total = 0;
                for (int i = 0; i < diffValues.Count; i++)
                {
                    total += diffValues[i] * Math.Pow(diff_weight, i);
                }
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

            // Decay excertion
            _excertion *= ExcertionDecay(GetDifficultyPoint(0).Time - GetDifficultyPoint(1)?.Time ?? 0);

            // Calculate difficulty of point
            double pointDiff = CalculateDiff();
            _diffList.Add(pointDiff);
        }

        /// <summary>
        /// Add <see cref="DifficultyPoint"/> to <see cref="_currentDiffPoints"/> and remove any now irrelevent points
        /// </summary>
        /// <param name="diffPoint"><see cref="DifficultyPoint"/> to add</param>
        protected abstract void UpdateDifficultyPoints(TDiffPoint diffPoint);

        /// <summary>
        /// Calculates difficulty based on <see cref="_currentDiffPoints"/>
        /// </summary>
        /// <returns>Total difficulty of the current object</returns>
        protected double CalculateDiff()
        {
            _excertion += CalculateRawDiff();
            return _excertion * CalculateBonusDiff();
        }

        /// <summary>
        /// Calculates the raw difficulty value which is added to <see cref="_excertion"/>
        /// </summary>
        /// <returns>Raw difficulty value of the current object</returns>
        protected abstract double CalculateRawDiff();

        /// <summary>
        /// Calculates the bonus difficulty multiplier which affects the value added to <see cref="_diffList"/>
        /// </summary>
        /// <returns>Bonus difficulty multiplier of the current object</returns>
        protected virtual double CalculateBonusDiff() => 1;

        /// <summary>
        /// Calculate multiplier to decay <see cref="_excertion"/> by
        /// </summary>
        /// <param name="time">Decay time</param>
        /// <returns>Amount decayed over time</returns>
        private double ExcertionDecay(double time) => Math.Pow(ExcertionDecayBase, time / 1000);

        public Skill(Mods mods = Mods.None)
        {
            _mods = mods;
        }
    }
}
