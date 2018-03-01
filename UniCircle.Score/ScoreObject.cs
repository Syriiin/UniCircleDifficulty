using System.Linq;
using System.Collections.Generic;

using UniCircleTools.Beatmaps;
using UniCircle.Difficulty.Skills;
using UniCircle.Difficulty.Standard.Skills.Physical.Aiming;
using UniCircle.Difficulty.Standard.Skills.Physical.Clicking;
using UniCircle.Difficulty.Standard.Skills.Reading;

namespace UniCircle.Score
{
    internal class ScoreObject
    {
        // ScoreObjects exist because HitObjects currently are defined in UniCircleTools (seperate solution)
        public HitObject BaseObject { get; }

        // In lazer implementation, these properties would be part of HitObject
        public List<DifficultyPoint> DifficultyPoints { get; } = new List<DifficultyPoint>();
        public double AimingDifficulty => DifficultyPoints.FindAll(p => p is AimPoint).Sum(p => p.Difficulty);
        public double ClickingDifficulty => DifficultyPoints.FindAll(p => p is ClickPoint).Sum(p => p.Difficulty);
        public double ReadingDifficulty => DifficultyPoints.FindAll(p => p is ReadingPoint).Sum(p => p.Difficulty);

        public ScoreObject(HitObject hitObject)
        {
            BaseObject = hitObject;
        }
    }
}
