using System;

using UniCircleTools.Beatmaps;

namespace UniCircle.Difficulty.Skills
{
    public abstract class DifficultyPoint
    {
        public HitObject BaseObject { get; set; }

        public double Difficulty { get; set; }

        public double Offset { get; set; }
        public double DeltaTime { get; set; }
    }
}
