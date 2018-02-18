using System;

using UniCircle.Difficulty.Skills;

namespace UniCircle.Difficulty.Standard.Skills.Visual
{
    public class ReadingPoint : DifficultyPoint, ICircle
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Radius { get; set; }
        public double ApproachTime { get; set; }

        public double FocalWeight { get; set; }
        public double RhythmicFocalWeight { get; set; }
    }
}
