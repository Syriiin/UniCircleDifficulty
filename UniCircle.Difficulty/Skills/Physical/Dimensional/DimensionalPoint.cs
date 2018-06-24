using System;

namespace UniCircle.Difficulty.Skills.Physical.Dimensional
{
    public abstract class DimensionalPoint : PhysicalPoint
    {
        public double TargetErrorRange { get; set; }
        public Vector Position { get; set; }
        public Vector IncomingForce { get; set; }

        // Data points
        public double SnapForceVolatility { get; set; }
        public double Snappiness { get; set; }
    }
}
