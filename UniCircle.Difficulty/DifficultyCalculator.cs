using System;
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

        protected List<ISkill> _skills = new List<ISkill>();

        public virtual double Difficulty => _skills.Sum(skill => skill.Value);

        public void SetBeatmap(Beatmap beatmap)
        {
            Reset();
            Beatmap = beatmap;
        }

        public void SetMods(Mods mods)
        {
            Reset();
            foreach (ISkill skill in _skills)
            {
                skill.SetMods(mods);
            }
        }

        public void Reset()
        {
            foreach (ISkill skill in _skills)
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
            foreach (ISkill skill in _skills)
            {
                skill.ProcessHitObjectSequence(Beatmap.HitObjects);
            }
        }
    }
}
