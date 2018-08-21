using System;
using System.Collections.Generic;

namespace UniCircle.Difficulty.Skills
{
    public class SkillData
    {
        private Type _skillType;
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

        public Dictionary<string, double> DataPoints { get; set; }
        
        public double Difficulty { get; set; }
    }
}
