using System.Collections.Generic;
using System.Linq;

using UniCircleTools;
using UniCircleTools.Beatmaps;

using UniCircle.Difficulty.Skills;

namespace UniCircle.Difficulty
{
    public abstract class DifficultyCalculator
    {
        public Beatmap Beatmap { get; private set; }

        protected List<ISkill> Skills = new List<ISkill>();

        public virtual double Difficulty => Skills.Sum(skill => skill.Value);

        public void SetBeatmap(Beatmap beatmap)
        {
            Reset();
            Beatmap = beatmap;
        }

        public void SetMods(Mods mods)
        {
            Reset();
            foreach (ISkill skill in Skills)
            {
                skill.SetMods(mods);
            }
        }

        public void Reset()
        {
            foreach (ISkill skill in Skills)
            {
                skill.Reset();
            }
        }

        public void CalculateDifficulty()
        {
            if (Beatmap == null)
            {
                return;
            }

            // Calculates skill difficulties
            foreach (ISkill skill in Skills)
            {
                skill.ProcessHitObjectSequence(Beatmap.HitObjects);
            }
        }
    }
}
