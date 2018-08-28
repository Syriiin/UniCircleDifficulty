using System.Collections.Generic;
using UniCircleTools.Beatmaps;

namespace UniCircle.Difficulty.Skills
{
    public interface ISkill
    {
        /// <summary>
        /// Processes <see cref="HitObject"/> and makes preperations before <see cref="CalculateDifficulty"/> is called
        /// </summary>
        /// <param name="hitObject"><see cref="HitObject"/> to process</param>
        void ProcessHitObject(HitObject hitObject);

        /// <summary>
        /// Calculates difficulty of <see cref="HitObject"/> passed in <see cref="ProcessHitObject"/>
        /// (any tasks that modify data such that calling this method a second time would yield different results should be done within <see cref="ProcessHitObject"/>)
        /// </summary>
        /// <returns>Calculated difficulty</returns>
        double CalculateDifficulty();

        /// <summary>
        /// Dictionary containing data points to later be analysed
        /// </summary>
        Dictionary<string, double> DataPoints { get; }

        /// <summary>
        /// Resets all skill values to their default state
        /// </summary>
        void Reset();
    }
}
