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
        public virtual double Difficulty => Skills.Sum(skill => skill.Value);

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
