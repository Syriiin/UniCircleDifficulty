using System;
using System.Collections.Generic;
using System.Text;

namespace UniCircleDifficulty.Skills.Physical.Aiming
{
    public class AimPoint : PhysicalPoint, ICircle
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Radius { get; set; }
    }
}
