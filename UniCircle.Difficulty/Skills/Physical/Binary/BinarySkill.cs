using System;

namespace UniCircle.Difficulty.Skills.Physical.Binary
{
    public abstract class BinarySkill : PhysicalSkill
    {
        protected override double CalculateEnergyExerted() => 1;
    }
}
