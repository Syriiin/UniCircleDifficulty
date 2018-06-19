namespace UniCircle.Difficulty.Skills.Physical
{
    public abstract class PhysicalPoint : DifficultyPoint
    {
        public double TimeErrorRange { get; set; }
        public double EnergyErrorRange { get; set; }

        // Data points

        /// <summary>
        /// Data point: Speed at the time of this difficulty point
        /// </summary>
        public double CurrentSpeed { get; set; }

        /// <summary>
        /// Data point: Stamina at the time of this difficulty point
        /// </summary>
        public double CurrentStamina { get; set; }
    }
}
