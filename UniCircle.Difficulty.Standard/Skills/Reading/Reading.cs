using System;
using System.Collections.Generic;
using System.Linq;

using UniCircleTools.Beatmaps;

using UniCircle.Difficulty.Skills;

namespace UniCircle.Difficulty.Standard.Skills.Reading
{
    /// <summary>
    /// Skill representing the difficulty of identifying rhythmic and visual patterns in notes
    /// </summary>
    public class Reading : ISkill
    {
        // Curve constants
        public double FocalDistanceThreshold { get; set; } = 100;
        public double FocalDistanceCurveHarshness { get; set; } = 0.05;  // Higher = quicker change

        public double OverlapThreshold { get; set; } = 25;
        public double OverlapCurveHarshness { get; set; } = 0.1;

        public double RhythmDistanceCurveHarshness { get; set; } = 7;
        public double RhythmDelayCurveHarshness { get; set; } = 10;

        private List<ReadingPoint> CurrentReadingPoints = new List<ReadingPoint>();

        // Shortcuts for readability
        private ReadingPoint ReadingPointA => CurrentReadingPoints[CurrentReadingPoints.Count - 1];
        private ReadingPoint ReadingPointB => CurrentReadingPoints[CurrentReadingPoints.Count - 2];
        private ReadingPoint ReadingPointC => CurrentReadingPoints[CurrentReadingPoints.Count - 3];

        private double FocalTotal => CurrentReadingPoints.Sum(rp => rp.FocalWeight);
        private double RhythmicFocalTotal => CurrentReadingPoints.Sum(rp => rp.RhythmicFocalWeight);

        public double AimReadingWeight { get; set; } = 1;
        public double RhythmicReadingWeight { get; set; } = 1;

        public Dictionary<string, double> DataPoints { get; set; } = new Dictionary<string, double>();

        public bool ProcessHitObject(HitObject hitObject)
        {
            if (hitObject is Spinner)
            {
                // Spinners are not considered in reading
                return false;
            }

            var readingPoint = new ReadingPoint
            {
                HitObject = hitObject
            };

            // Construct visual points from hitobject and call ProcessDifficultyPoint with them
            UpdateDifficultyPoints(readingPoint);

            return true;
        }

        private void UpdateDifficultyPoints(ReadingPoint readingPoint)
        {
            // Add readingPoint to currentDiffPoints
            CurrentReadingPoints.Add(readingPoint);

            // Remove points that are no longer visible
            double currentTime = readingPoint.HitObject.Time - readingPoint.HitObject.ApproachTime;   // Visual points are processed at the moment they first appear
            CurrentReadingPoints.RemoveAll(rp => rp.HitObject.Time < currentTime);
        }

        public double CalculateDifficulty()
        {
            if (CurrentReadingPoints.Count < 2)   // Not enough visual points to cause difficulty
            {
                return 0;
            }

            var readingPoint = ReadingPointA;

            double aimReading = AimReading() * AimReadingWeight;
            double rhythmicReading = RhythmicReading() * RhythmicReadingWeight;
            double speedBonus = SpeedBonus(readingPoint.HitObject.ApproachTime);

            // Data Points
            DataPoints.Add("Aim Reading", aimReading);
            DataPoints.Add("Rhythmic Reading", rhythmicReading);
            DataPoints.Add("Speed bonus", speedBonus);

            return speedBonus * (aimReading + rhythmicReading);
        }

        /// <summary>
        /// Difficulty of determining the position and order of aim points
        /// </summary>
        /// <returns>Difficulty as a double</returns>
        private double AimReading()
        {
            // Step 1: Assign point a focal weight
            ReadingPointA.FocalWeight = FocalWeight(ReadingPointA.HitObject, ReadingPointB.HitObject);

            // Step 2: Search for nearest note
            var nearestPoint = CurrentReadingPoints.Take(CurrentReadingPoints.Count - 1).OrderBy(rp => NormalisedDistance(ReadingPointA.HitObject, rp.HitObject)).FirstOrDefault();

            // Step 3: Calculate overlap bonus
            double overlapBonus = OverlapBonus(ReadingPointA.HitObject, nearestPoint.HitObject);
            
            return FocalTotal * ReadingPointA.FocalWeight * overlapBonus;
        }

        // Focal weight of visual point A. Scales from 0 to 1 with distance
        private double FocalWeight(HitObject hitObjectA, HitObject hitObjectB)
        {
            double distance = NormalisedDistance(hitObjectA, hitObjectB);

            return Math.Tanh((distance - FocalDistanceThreshold) * FocalDistanceCurveHarshness) / 2 + 0.5;
        }

        private double OverlapBonus(HitObject hitObjectA, HitObject hitObjectB)
        {
            if (hitObjectB == null)
            {
                return 1;
            }

            double distance = NormalisedDistance(hitObjectA, hitObjectB);

            return Math.Tanh((distance - OverlapThreshold) * OverlapCurveHarshness) / -2 + 1.5;
        }

        /// <summary>
        /// Difficulty of determining the timing of click points.
        /// Mostly only relevent for old maps without general time/distance equality.
        /// </summary>
        /// <returns>Difficulty as a double</returns>
        private double RhythmicReading()
        {
            // Rhythmic reading deals with timing changes where there is almost no distance change
            // Timing changes without distance changes (accounts for overlapping notes with varying timings)

            // NOTE: I am unsure if this fits into the idea of map difficulty since identifying rhythm is intended to be done through music. 
            //          However time-distance proportions are something that are a staple in almost all maps; so I'm not really sure if it should be relevent or not.
            
            if (CurrentReadingPoints.Count < 3)   // Not enough visual points to cause rhythmic difficulty
            {
                return 0;
            }

            // Calculate and set rhythmic focal weight of the visual point
            ReadingPointA.RhythmicFocalWeight = RhythmicFocalWeight(ReadingPointA.HitObject, ReadingPointB.HitObject, ReadingPointC.HitObject);

            // TODO: Currently notes that have a back forth where the middle note is a slider cause false positives for timing change since slider bodies are ignored.
            //          To fix, could give ReadingPoints an end point that would be the same as start point for normal circles, but coords of slidertail for sliders

            return RhythmicFocalTotal * ReadingPointA.RhythmicFocalWeight;
        }

        private double RhythmicFocalWeight(HitObject hitObjectA, HitObject hitObjectB, HitObject hitObjectC)
        {
            // Step 1: Determine distance change weight (low distance change = high, decreases and drops off exponentially)
            double distanceAB = NormalisedDistance(hitObjectA, hitObjectB);
            double distanceBC = NormalisedDistance(hitObjectB, hitObjectC);
            double distanceChangeWeight = DistanceChangeWeight(distanceAB, distanceBC);

            // Step 2: Determine timing change weight (low timing change = low, increases and caps out exponentially)
            double delayAB = hitObjectA.Time - hitObjectB.Time;
            double delayBC = hitObjectB.Time - hitObjectC.Time;
            double delayChangeWeight = DelayChangeWeight(delayAB, delayBC);

            // Step 3: Return diff
            return distanceChangeWeight * delayChangeWeight;
        }

        // Defines how much the distance change between 3 points affects timing reading
        private double DistanceChangeWeight(double distanceAB, double distanceBC)
        {
            // Domain: [1, infinity]
            // Range: [0, 1]
            // TODO: Contemplate using absolute distances instead of ratio. Since distance changes effects are less the further they are from each other
            double distanceChange = Math.Min(distanceAB, distanceBC) == 0 ? double.PositiveInfinity : Math.Max(distanceAB, distanceBC) / Math.Min(distanceAB, distanceBC);
            return Math.Pow(Math.E, -RhythmDistanceCurveHarshness * (distanceChange - 1));
        }

        // Defines how much the delay change between 3 points affects timing reading
        private double DelayChangeWeight(double delayAB, double delayBC)
        {
            // Domain: [1, infinity]
            // Range: [0, 1]
            double delayChange = Math.Max(delayAB, delayBC) / Math.Min(delayAB, delayBC);
            if (Math.Abs(delayAB - delayBC) < 3)   // Integer millisecond snapping error
            {
                delayChange = 1;
            }
            return -Math.Pow(Math.E, -RhythmDelayCurveHarshness * (delayChange - 1)) + 1;
        }

        private double SpeedBonus(double approachTime)
        {
            // 2x bonus at ar11
            // 1.082x bonus at ar10.3
            return Math.Pow(Math.E, (-approachTime + 300) / 40) + 1;
        }

        /// <summary>
        /// Calculate normalised distance of 2 circles with circle radius 52
        /// </summary>
        /// <param name="hitObjectA">1st circle</param>
        /// <param name="hitObjectB">2nd circle</param>
        /// <returns>Normalised distance</returns>
        private static double NormalisedDistance(HitObject hitObjectA, HitObject hitObjectB)
        {
            // Average CS (to support possible lazer variable CS) 
            double avgRadius = (hitObjectB.Radius + hitObjectA.Radius) / 2;
            // Ratio of distance to CS
            double distanceRatio = Utils.Distance(hitObjectB, hitObjectA) / avgRadius;
            // Normalised distance at radius 52
            return distanceRatio * 52;
        }

        public void Reset()
        {
            CurrentReadingPoints.Clear();
        }
    }
}
