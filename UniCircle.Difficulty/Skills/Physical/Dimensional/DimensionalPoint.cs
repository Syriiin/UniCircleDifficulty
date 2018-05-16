using System;

namespace UniCircle.Difficulty.Skills.Physical.Dimensional
{
    public abstract class DimensionalPoint : PhysicalPoint
    {
        public Vector Position { get; set; }
        public Vector IncomingForce { get; set; }
    }
}
