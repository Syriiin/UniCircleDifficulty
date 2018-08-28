using System;
using System.Collections.Generic;

namespace UniCircle.Difficulty.Skills
{
    /// <summary>
    /// Container for difficulty and other data relating to a specific <see cref="UniCircleTools.Beatmaps.HitObject"/> in the context of an <see cref="ISkill"/>
    /// </summary>
    public class SkillData
    {
        private Type _skillType;
        
        /// <summary>
        /// Type of <see cref="ISkill"/> this container's data is from
        /// </summary>
        public Type SkillType
        {
            get => _skillType;
            internal set
            {
                if (typeof(ISkill).IsAssignableFrom(value))
                {
                    _skillType = value;
                }
            }
        }

        /// <summary>
        /// Dictionary containing data points to later be analysed
        /// </summary>
        public Dictionary<string, double> DataPoints { get; set; }
        
        /// <summary>
        /// Calculated difficulty of latest <see cref="UniCircleTools.Beatmaps.HitObject"/> processed by the <see cref="ISkill"/>
        /// </summary>
        public double Difficulty { get; set; }
    }
}
