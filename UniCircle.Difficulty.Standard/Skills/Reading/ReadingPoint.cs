using UniCircleTools.Beatmaps;

namespace UniCircle.Difficulty.Standard.Skills.Reading
{
    /// <summary>
    /// Data class used for holding reading data on currently visable <see cref="HitObject"/>
    /// </summary>
    public class ReadingPoint
    {
        public HitObject HitObject { get; set; }

        public double FocalWeight { get; set; }
        public double RhythmicFocalWeight { get; set; }
    }
}
