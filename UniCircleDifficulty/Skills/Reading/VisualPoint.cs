using System;
using System.Collections.Generic;
using System.Text;

namespace UniCircleDifficulty.Skills.Reading
{
    class VisualPoint : DifficultyPoint, ICircle
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Radius { get; set; }
    }
}
