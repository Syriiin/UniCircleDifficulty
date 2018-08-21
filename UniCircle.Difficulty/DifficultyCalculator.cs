using System;
using System.Collections.Generic;
using System.Linq;

using UniCircleTools;
using UniCircleTools.Beatmaps;

using UniCircle.Difficulty.Skills;

namespace UniCircle.Difficulty
{
    /// <summary>
    /// Base class for creating osu! difficulty calculators
    /// </summary>
    public abstract class DifficultyCalculator
    {
        /// <summary>
        /// List of skills this calculator is processing with
        /// </summary>
        protected readonly List<ISkill> Skills = new List<ISkill>();

        public readonly List<DifficultyPoint> DifficultyPoints = new List<DifficultyPoint>();

        /// <summary>
        /// <see cref="UniCircleTools.Beatmaps.Beatmap"/> to calculator difficulty for
        /// </summary>
        public Beatmap Beatmap { get; private set; }

        public Mods Mods { get; private set; }

        /// <summary>
        /// Calculated difficulty of beatmap
        /// </summary>
        public double Difficulty
        {
            get
            {
                // Get difficulty values
                var difficulties = DifficultyPoints.Select(d => d.Difficulty);

                // Order difficulties
                difficulties = difficulties.OrderByDescending(d => d).ToList();

                // Determine beatmap difficulty
                double total = difficulties.Max();  // Literal *difficulty* (not performance required) of a map is the difficulty of its most difficulty point

                // Apply difficulty curve
                return Math.Sqrt(total);
            }
        }

        /// <summary>
        /// Sets the <see cref="Beatmap"/> to be calculated
        /// </summary>
        /// <param name="beatmap"></param>
        public void SetBeatmap(Beatmap beatmap)
        {
            Reset();
            Beatmap = beatmap;
        }

        /// <summary>
        /// Sets the <see cref="Mods"/> to apply to the <see cref="Beatmap"/>
        /// </summary>
        /// <param name="mods"></param>
        public void SetMods(Mods mods)
        {
            Reset();
            Mods = mods;
        }

        /// <summary>
        /// Clears skills of calculation data
        /// </summary>
        public void Reset()
        {
            foreach (var skill in Skills)
            {
                skill.Reset();
            }
        }

        /// <summary>
        /// Calculates difficulty of current <see cref="Beatmap"/> with <see cref="Mods"/> set
        /// </summary>
        public void CalculateDifficulty()
        {
            if (Beatmap == null)
            {
                return;
            }

            // Process HitObjects
            foreach (var hitObject in Beatmap.HitObjects)
            {
                var difficultyPoint = new DifficultyPoint(hitObject);

                foreach (var skill in Skills)
                {
                    skill.ProcessHitObject(HitObjectWithMods(difficultyPoint.BaseHitObject, Mods));

                    difficultyPoint.SkillDatas.Add(new SkillData
                    {
                        SkillType = skill.GetType(),
                        Difficulty = skill.CalculateDifficulty(),
                        DataPoints = skill.DataPoints
                    });
                }

                DifficultyPoints.Add(difficultyPoint);
            }
        }

        protected abstract HitObject HitObjectWithMods(HitObject hitObject, Mods mods);
    }
}
