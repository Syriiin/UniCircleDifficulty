using System;

namespace UniCircle.Difficulty.Skills.Physical.Binary
{
    public abstract class BinarySkill<TDiffPoint> : PhysicalSkill<TDiffPoint> where TDiffPoint : BinaryPoint
    {
        protected override double CalculateEnergyExerted() => 1;
    }
}
