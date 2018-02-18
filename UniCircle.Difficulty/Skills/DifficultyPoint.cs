using System;

namespace UniCircleDifficulty.Skills
{
    public abstract class DifficultyPoint
    {
        public double Difficulty { get; set; }

        public double Offset { get; set; }
        public double DeltaTime { get; set; }
    }
}
