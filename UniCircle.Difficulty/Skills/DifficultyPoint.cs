using UniCircleTools.Beatmaps;

namespace UniCircle.Difficulty.Skills
{
    /// <summary>
    /// Represents a point of difficulty relevent to a specific skill
    /// </summary>
    public abstract class DifficultyPoint
    {
        /// <summary>
        /// <see cref="HitObject"/> this difficulty point belongs to
        /// </summary>
        public HitObject BaseObject { get; set; }

        /// <summary>
        /// Calculated difficulty of this point
        /// </summary>
        public double Difficulty { get; set; }

        /// <summary>
        /// The time in the beatmap that this difficulty point occurs
        /// </summary>
        public double Offset { get; set; }

        /// <summary>
        /// The elapsed time since the previous difficulty point of the same type, or 0 if there are no none
        /// </summary>
        public double DeltaTime { get; set; }
    }
}
