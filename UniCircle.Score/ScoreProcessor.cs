using System;
using System.Linq;
using System.Collections.Generic;

using UniCircleTools;
using UniCircleTools.Beatmaps;
using UniCircle.Difficulty.Skills;
using UniCircle.Difficulty.Standard;

namespace UniCircle.Score
{
    public enum HitJudgement
    {
        Hit300,
        Hit100,
        Hit50,
        Miss,
        Sliderbreak
    }

    public class ScoreProcessor
    {
        public double Score { get; private set; }
        public int Combo { get; set; }

        private List<ScoreObject> _remainingScoreObjects = new List<ScoreObject>();

        private Beatmap _beatmap;
        private Mods _mods;

        public ScoreProcessor(Beatmap beatmap, Mods mods)
        {
            _beatmap = beatmap;
            _mods = mods;
            ProcessBeatmap();
        }

        public void ApplyHitJudgement(HitJudgement judgement)
        {
            ScoreObject currentObject = _remainingScoreObjects.FirstOrDefault();
            if (currentObject == null)
            {
                return;
            }
            _remainingScoreObjects.RemoveAt(0);
            
            double difficultyMultiplier = currentObject.AimingDifficulty + currentObject.ClickingDifficulty + currentObject.ReadingDifficulty;
            // Perhaps change later such that different difficulty sources only give bonus for certain results
            //  aim => if not break (not miss or sliderbreak)
            //  click => if not miss (scales with hitvalue)
            //  read => always (scales with hitvalue)

            double hitValue = 0;

            switch (judgement)
            {
                case HitJudgement.Hit300:
                    hitValue = 300;
                    break;
                case HitJudgement.Hit100:
                    hitValue = 100;
                    break;
                case HitJudgement.Hit50:
                    hitValue = 50;
                    break;
                case HitJudgement.Sliderbreak:
                    hitValue = 100;
                    break;
            }

            Combo++;

            // Mods multiplier is considered in difficulty multiplier
            Score += hitValue + hitValue * difficultyMultiplier * Combo / 25;
        }

        private void ProcessBeatmap()
        {
            // In a lazer implementation, DifficultyPoints would be a property of HitObjects so
            //  there would be no need to recalculate the difficulty or do this messy sorting
            DifficultyCalculator calculator = new DifficultyCalculator();
            calculator.SetBeatmap(_beatmap);
            calculator.SetMods(_mods);
            calculator.CalculateDifficulty();
            
            foreach (DifficultyPoint diffPoint in calculator.Aiming.CalculatedPoints)
            {
                if (_remainingScoreObjects.Find(s => s.BaseObject == diffPoint.BaseObject) == null)
                {
                    _remainingScoreObjects.Add(new ScoreObject(diffPoint.BaseObject));
                }

                _remainingScoreObjects.Find(s => s.BaseObject == diffPoint.BaseObject).DifficultyPoints.Add(diffPoint);
            }

            foreach (DifficultyPoint diffPoint in calculator.Clicking.CalculatedPoints)
            {
                if (_remainingScoreObjects.Find(s => s.BaseObject == diffPoint.BaseObject) == null)
                {
                    _remainingScoreObjects.Add(new ScoreObject(diffPoint.BaseObject));
                }

                _remainingScoreObjects.Find(s => s.BaseObject == diffPoint.BaseObject).DifficultyPoints.Add(diffPoint);
            }

            foreach (DifficultyPoint diffPoint in calculator.Reading.CalculatedPoints)
            {
                if (_remainingScoreObjects.Find(s => s.BaseObject == diffPoint.BaseObject) == null)
                {
                    _remainingScoreObjects.Add(new ScoreObject(diffPoint.BaseObject));
                }

                _remainingScoreObjects.Find(s => s.BaseObject == diffPoint.BaseObject).DifficultyPoints.Add(diffPoint);
            }

            _remainingScoreObjects.OrderBy(s => s.BaseObject.Time);
        }
    }
}
