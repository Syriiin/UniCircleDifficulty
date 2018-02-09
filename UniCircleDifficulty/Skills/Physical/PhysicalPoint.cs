using System;

namespace UniCircleDifficulty.Skills.Physical
{
    public abstract class PhysicalPoint : DifficultyPoint
    {
        // Data points
        public double CurrentSpeed { get; set; }
        public double CurrentStamina { get; set; }
    }
}
