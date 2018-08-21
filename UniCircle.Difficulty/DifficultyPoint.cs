using System.Collections.Generic;
using System.Linq;
using UniCircleTools.Beatmaps;

using UniCircle.Difficulty.Skills;

namespace UniCircle.Difficulty
{
    public class DifficultyPoint
    {
        public HitObject BaseHitObject { get; }
        public List<SkillData> SkillDatas { get; } = new List<SkillData>();
        public double Difficulty => SkillDatas.Sum(d => d.Difficulty);

        public DifficultyPoint(HitObject baseHitObject)
        {
            BaseHitObject = baseHitObject;
        }
    }
}
