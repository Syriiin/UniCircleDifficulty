using System.Collections.Generic;
using UniCircleTools.Beatmaps;

namespace UniCircle.Difficulty.Skills
{
    public interface ISkill
    {
        void ProcessHitObject(HitObject hitObject);
        double CalculateDifficulty();
        Dictionary<string, double> DataPoints { get; }
        void Reset();
    }
}
