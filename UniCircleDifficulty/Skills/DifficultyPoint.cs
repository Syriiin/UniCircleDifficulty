using System;
using System.Collections.Generic;
using System.Text;

namespace UniCircleDifficulty
{
    class DifficultyPoint
    {
        public double EnergyExerted { get; set; }
        public double BonusMultiplier { get; set; }
        public double PriorExertion { get; set; }
        // Experimental deltatime squaring since exertion doesnt include time anymore
        public double Difficulty => EnergyExerted / Math.Pow(DeltaTime, 2) * BonusMultiplier;

        public double Offset { get; set; }
        public double DeltaTime { get; set; }
    }
}
