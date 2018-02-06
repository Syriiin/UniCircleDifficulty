using System;
using System.Collections.Generic;
using System.Text;

namespace UniCircleDifficulty.Skills.Reading
{
    public class VisualPoint : DifficultyPoint, ICircle
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Radius { get; set; }
        public double ApproachTime { get; set; }

        public double FocalWeight { get; set; }
        public double RhythmicFocalWeight { get; set; }
    }
}
