using System;
using System.Collections.Generic;
using System.Text;

namespace UniCircleDifficulty.Skills.Aiming
{
    class AimPoint : DifficultyPoint, ICircle
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Radius { get; set; }
    }
}
