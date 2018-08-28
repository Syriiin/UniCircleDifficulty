using System;

namespace UniCircle.Difficulty.Skills.Physical.Binary
{
    /// <summary>
    /// Abstract class to assist when creating a skill centered around binary physical exertion (eg. clicking)
    /// </summary>
    public abstract class BinarySkill : PhysicalSkill
    {
        /// <inheritdoc />
        protected override double CalculateEnergyExerted() => 1;
    }
}
