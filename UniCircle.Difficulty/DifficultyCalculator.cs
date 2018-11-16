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
        /// List of <see cref="ISkill"/> this calculator is processing objects with
        /// </summary>
        public List<ISkill> Skills { get; } = new List<ISkill>();

        /// <summary>
        /// List of <see cref="DifficultyPoint"/> this calculator has processed
        /// </summary>
        public List<DifficultyPoint> DifficultyPoints { get; } = new List<DifficultyPoint>();

        /// <summary>
        /// <see cref="UniCircleTools.Beatmaps.Beatmap"/> to calculator difficulty for
        /// </summary>
        public Beatmap Beatmap { get; private set; }

        /// <summary>
        /// <see cref="UniCircleTools.Mods"/> to apply to <see cref="HitObject"/>
        /// </summary>
        public Mods Mods { get; private set; } = Mods.None;

        /// <summary>
        /// Calculated difficulty of beatmap
        /// </summary>
        public double Difficulty
        {
            get
            {
                // Determine beatmap difficulty
                // Literal *difficulty* (not necessarily performance required) of a map is the difficulty of its most difficulty point
                double total = DifficultyPoints.Max(d => d.Difficulty);

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
        /// Clears <see cref="Skills"/> of calculation data
        /// </summary>
        public void Reset()
        {
            DifficultyPoints.Clear();
            foreach (var skill in Skills)
            {
                skill.Reset();
                skill.DataPoints = null;
            }
        }

        /// <summary>
        /// Calculates difficulty of current <see cref="Beatmap"/> with <see cref="Mods"/> set
        /// </summary>
        public void CalculateDifficulty()
        {
            // Check for beatmap
            if (Beatmap == null)
            {
                return;
            }

            // Reset skills
            Reset();

            // Process HitObjects
            foreach (var hitObject in Beatmap.HitObjects)
            {
                var difficultyPoint = new DifficultyPoint(hitObject);

                foreach (var skill in Skills)
                {
                    if (skill.ProcessHitObject(HitObjectWithMods(difficultyPoint.BaseHitObject, Mods)))
                    {
                        skill.DataPoints = new Dictionary<string, double>();
                        difficultyPoint.SkillDatas.Add(new SkillData
                        {
                            SkillType = skill.GetType(),
                            Difficulty = skill.CalculateDifficulty(),
                            DataPoints = skill.DataPoints
                        });
                    }
                }

                DifficultyPoints.Add(difficultyPoint);
            }
        }

        /// <summary>
        /// Helper method for applying <see cref="UniCircleTools.Mods"/> to <see cref="HitObject"/>
        /// (this should really be part of <see cref="HitObject"/> itself, but isn't for now)
        /// </summary>
        /// <param name="baseHitObject">Base <see cref="HitObject"/></param>
        /// <param name="mods"><see cref="UniCircleTools.Mods"/> to apply</param>
        /// <returns><see cref="HitObject"/> with <see cref="UniCircleTools.Mods"/> applied</returns>
        protected abstract HitObject HitObjectWithMods(HitObject baseHitObject, Mods mods);
    }
}
