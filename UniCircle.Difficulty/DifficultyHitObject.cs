using System;
using System.Collections.Generic;

using UniCircleTools.Beatmaps;

using UniCircle.Difficulty.Skills;

namespace UniCircle.Difficulty
{
    /// <summary>
    /// A simple wrapper class for HitObjects so we can group related skill data with the relevent HitObject
    /// </summary>
    public class DifficultyHitObject
    {
        public HitObject BaseHitObject { get; }
        public List<DifficultyPoint> DifficultyPoints { get; } = new List<DifficultyPoint>();

        public DifficultyHitObject(HitObject baseHitObject)
        {
            BaseHitObject = baseHitObject;
        }
    }
}
