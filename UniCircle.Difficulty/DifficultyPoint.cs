using System.Collections.Generic;
using System.Linq;

using UniCircleTools.Beatmaps;

using UniCircle.Difficulty.Skills;

namespace UniCircle.Difficulty
{
    /// <summary>
    /// Container for <see cref="HitObject"/> that holds related <see cref="SkillData"/>
    /// </summary>
    public class DifficultyPoint
    {
        /// <summary>
        /// Base <see cref="HitObject"/> being wrapped
        /// </summary>
        public HitObject BaseHitObject { get; }
        
        /// <summary>
        /// List of <see cref="SkillData"/> related to this DifficultyPoint
        /// </summary>
        public List<SkillData> SkillDatas { get; } = new List<SkillData>();

        /// <summary>
        /// Calculated difficulty of this DifficultyPoint
        /// </summary>
        public double Difficulty => SkillDatas.Sum(d => d.Difficulty);

        public DifficultyPoint(HitObject baseHitObject)
        {
            BaseHitObject = baseHitObject;
        }
    }
}
