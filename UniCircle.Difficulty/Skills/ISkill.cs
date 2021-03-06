﻿using System.Collections.Generic;

using UniCircleTools.Beatmaps;

namespace UniCircle.Difficulty.Skills
{
    public interface ISkill
    {
        /// <summary>
        /// Checks if <see cref="HitObject"/> is relevent to the skill and makes preperations before <see cref="CalculateDifficulty"/> is called
        /// </summary>
        /// <param name="hitObject"><see cref="HitObject"/> to process</param>
        /// <returns>Bool indicating if HitObject is relevent to this skill</returns>
        bool ProcessHitObject(HitObject hitObject);

        /// <summary>
        /// Calculates difficulty of <see cref="HitObject"/> passed in <see cref="ProcessHitObject"/>
        /// (any tasks that modify data such that calling this method a second time would yield different results should be done within <see cref="ProcessHitObject"/>)
        /// </summary>
        /// <returns>Calculated difficulty</returns>
        double CalculateDifficulty();

        /// <summary>
        /// Dictionary containing data points to later be analysed
        /// </summary>
        Dictionary<string, double> DataPoints { get; set; }

        /// <summary>
        /// Resets all skill values to their default state
        /// </summary>
        void Reset();
    }
}
