using System;
using System.Collections.Generic;
using UniCircle.Difficulty.Skills;
using UniCircle.Difficulty.Standard.Skills;
using UniCircle.Difficulty.Standard.Skills.Physical.Aiming;
using UniCircle.Difficulty.Standard.Skills.Physical.Clicking;
using UniCircle.Difficulty.Standard.Skills.Reading;

using UniCircleTools;
using UniCircleTools.Beatmaps;

namespace UniCircle.Difficulty.Standard
{
    /// <summary>
    /// Difficulty calculator for osu!standard
    /// </summary>
    public class DifficultyCalculator : Difficulty.DifficultyCalculator
    {
        public DifficultyCalculator()
        {
            Skills.AddRange(new ISkill[]
            {
                new Aiming(),
                new Clicking(),
                new Reading(),
            });
        }

        /// <inheritdoc />
        protected override HitObject HitObjectWithMods(HitObject baseHitObject, Mods mods)
        {
            var hitObject = Activator.CreateInstance(baseHitObject.GetType()) as HitObject;

            hitObject.Time = (int)Math.Round(baseHitObject.Time / Utils.ModClockRate(mods));
            hitObject.X = baseHitObject.X;
            hitObject.Y = mods.HasFlag(Mods.HardRock) ? -baseHitObject.Y + 384 : baseHitObject.Y;
            hitObject.NewCombo = baseHitObject.NewCombo;
            hitObject.Difficulty = new BeatmapDifficulty
            {
                AR = Utils.ModApproachRate(baseHitObject.Difficulty.AR, mods),
                CS = Utils.ModCircleSize(baseHitObject.Difficulty.CS, mods),
                HP = baseHitObject.Difficulty.HP,   // not important
                OD = Utils.ModOverallDifficulty(baseHitObject.Difficulty.OD, mods),
                SliderMultiplier = baseHitObject.Difficulty.SliderMultiplier,   // not important yet
                SliderTickRate = baseHitObject.Difficulty.SliderTickRate,   // not important yet
            };

            if (hitObject is Slider)
            {
                foreach (var curvePoint in ((Slider)baseHitObject).CurvePoints.ToArray())
                {
                    ((Slider)baseHitObject).CurvePoints.Add(new CurvePoint
                    {
                        type = curvePoint.type,
                        x = curvePoint.x,
                        y = mods.HasFlag(Mods.HardRock) ? -curvePoint.y + 384 : curvePoint.y
                    });
                }
                ((Slider)hitObject).EndTime = (int)Math.Round(((Slider)baseHitObject).EndTime / Utils.ModClockRate(mods));
                ((Slider)hitObject).PixelLength = ((Slider)baseHitObject).PixelLength;
                ((Slider)hitObject).Repeat = ((Slider)baseHitObject).Repeat;
                ((Slider)hitObject).SliderType = ((Slider)baseHitObject).SliderType;
            }

            if (hitObject is Spinner)
            {
                ((Spinner)hitObject).EndTime = (int)Math.Round(((Spinner)baseHitObject).EndTime / Utils.ModClockRate(mods));
            }

            return hitObject;
        }
    }
}
