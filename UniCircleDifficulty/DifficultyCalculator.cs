using System;

using UniCircleTools;
using UniCircleTools.Beatmaps;

namespace UniCircleDifficulty
{
    public class DifficultyCalculator
    {
        private Beatmap _beatmap;
        private Mods _mods;
        
        public DifficultyCalculator(Beatmap beatmap, Mods mods = Mods.None)
        {
            _beatmap = beatmap;
            _mods = mods;
            PreprocessBeatmap();
        }

        public double CalculateDifficulty()
        {
            // Calculates skill difficulties
            Aim aim = new Aim();
            Speed speed = new Speed();
            Accuracy accuracy = new Accuracy();
            Reading reading = new Reading();

            //aim.ProcessHitObjectSequence(_beatmap.HitObjects);
            speed.ProcessHitObjectSequence(_beatmap.HitObjects);

            return aim.Value + speed.Value + accuracy.Value + reading.Value;
        }

        private void PreprocessBeatmap()
        {
            // Modify beatmap in alignment with map changing mods
        }
    }
}
