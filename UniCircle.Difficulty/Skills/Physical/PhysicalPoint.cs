using System;

namespace UniCircle.Difficulty.Skills.Physical
{
    public abstract class PhysicalPoint : DifficultyPoint
    {
        // Data points
        public double CurrentSpeed { get; set; }
        public double CurrentStamina { get; set; }
    }
}
