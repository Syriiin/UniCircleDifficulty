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
        protected List<ISkill> Skills = new List<ISkill>();

        /// <summary>
        /// <see cref="UniCircleTools.Beatmaps.Beatmap"/> to calculator difficulty for
        /// </summary>
        public Beatmap Beatmap { get; private set; }

        /// <summary>
        /// Calculated difficulty of beatmap
        /// </summary>
        public double Difficulty
        {
            get
            {
                // TODO: refactor out of difficulty point model since we want each hit object to have only 1 difficulty point per skill
                //  (might not need to refactor entirly. just enforce single difficulty points)
                //  - would be better to have a list of objects in this class that held references to the specific skill difficulty points or something

                // Add all skills' difficulty points
                var difficulties = Skills[0].CalculatedDifficulties;
                foreach (var skill in Skills.Skip(1))
                {
                    for (int i = 1; i < difficulties.Count; i++)
                    {
                        difficulties[i] += skill.CalculatedDifficulties[i];
                    }
                }

                // Order difficulties
                difficulties = difficulties.OrderByDescending(d => d).ToList();

                // Determine beatmap difficulty
                double total = difficulties.Max();  // Literal *difficulty* (not performance required) of a map is the difficulty of its most difficulty point

                // Apply difficulty curve and normalise with multiplier
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
            foreach (var skill in Skills)
            {
                skill.SetMods(mods);
            }
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

            // Calculates skill difficulties
            foreach (var skill in Skills)
            {
                skill.ProcessHitObjectSequence(Beatmap.HitObjects);
            }
        }
    }
}
