using System;

using UniCircle.Difficulty.Skills.Physical;

namespace UniCircle.Difficulty.Standard.Skills.Physical.Aiming
{
    public class AimPoint : PhysicalPoint, ICircle
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Radius { get; set; }
    }
}
