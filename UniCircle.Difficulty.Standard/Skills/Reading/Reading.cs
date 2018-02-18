using System;
using System.Linq;

using UniCircle.Difficulty.Skills;
using UniCircleTools;
using UniCircleTools.Beatmaps;

namespace UniCircle.Difficulty.Standard.Skills.Visual
{
    /// <summary>
    /// Skill representing the difficulty of identifying rhythmic and visual patterns in notes
    /// </summary>
    public class Reading : Skill<ReadingPoint>
    {
        // Curve constants
        public double FocalDistanceThreshold { get; set; } = 100;
        public double FocalDistanceCurveHarshness { get; set; } = 0.05;  // Higher = quicker change

        public double OverlapThreshold { get; set; } = 25;
        public double OverlapCurveHarshness { get; set; } = 0.1;

        public double RhythmDistanceCurveHarshness { get; set; } = 7;
        public double RhythmDelayCurveHarshness { get; set; } = 10;

        // Shortcuts for readability
        private ReadingPoint ReadingPointA => GetDifficultyPoint(0);
        private ReadingPoint ReadingPointB => GetDifficultyPoint(1);
        private ReadingPoint ReadingPointC => GetDifficultyPoint(2);

        private double FocalTotal => _currentDiffPoints.Sum(rp => rp.FocalWeight);
        private double RhythmicFocalTotal => _currentDiffPoints.Sum(rp => rp.RhythmicFocalWeight);

        public double AimReadingWeight { get; set; } = 1;
        public double RhythmicReadingWeight { get; set; } = 1;

        public override double SkillMultiplier { get; set; } = 0.1;

        public override void ProcessHitObject(HitObject hitObject)
        {
            if (hitObject is Spinner)
            {
                // Spinners are not considered in reading
                return;
            }

            // Construct visual points from hitobject and call ProcessDifficultyPoint with them
            ReadingPoint readingPoint = new ReadingPoint
            {
                Offset = hitObject.Time / Utils.ModClockRate(_mods),
                ApproachTime = Utils.ModApproachTime(hitObject.Difficulty.AR, _mods) / Utils.ModClockRate(_mods),
                X = hitObject.X,
                Y = hitObject.Y,
                Radius = Utils.ModRadius(hitObject.Difficulty.CS, _mods)
            };

            if (_mods.HasFlag(Mods.HardRock))
            {
                readingPoint.Y = -readingPoint.Y + 384; // Flip notes (even though it technically doesnt matter since EVERYTHING is flipped)
            }

            // TODO: consider reading implications of sliders (create visual points at corners in curve?)

            ProcessDifficultyPoint(readingPoint);
        }

        protected override void UpdateDifficultyPoints(ReadingPoint readingPoint)
        {
            // Add readingPoint to currentDiffPoints
            _currentDiffPoints.Add(readingPoint);

            // Remove points that are no longer visible
            double currentTime = readingPoint.Offset - readingPoint.ApproachTime;   // Visual points are processed at the moment they first appear
            _currentDiffPoints.RemoveAll(rp => rp.Offset < currentTime);
        }

        protected override void CalculateDifficulty()
        {
            ReadingPoint readingPoint = ReadingPointA;

            if (_currentDiffPoints.Count == 1)   // Not enough visual points to cause difficulty
            {
                readingPoint.Difficulty = 0;
                return;
            }

            double aimReading = AimReading() * AimReadingWeight;
            double rhythmicReading = RhythmicReading() * RhythmicReadingWeight;
            double speedBonus = SpeedBonus(readingPoint.ApproachTime);

            readingPoint.Difficulty = speedBonus * (aimReading + rhythmicReading);
        }

        /// <summary>
        /// Difficulty of determining the position and order of aim points
        /// </summary>
        /// <returns>Difficulty as a double</returns>
        private double AimReading()
        {
            // Step 1: Assign point a focal weight
            ReadingPointA.FocalWeight = FocalWeight(ReadingPointA, ReadingPointB);

            // Step 2: Search for nearest note
            ReadingPoint nearestPoint = _currentDiffPoints.Take(_currentDiffPoints.Count - 1).OrderBy(rp => Utils.NormalisedDistance(rp, ReadingPointA)).FirstOrDefault();

            // Step 3: Calculate overlap bonus
            double overlapBonus = OverlapBonus(ReadingPointA, nearestPoint);
            
            return FocalTotal * ReadingPointA.FocalWeight * overlapBonus;
        }

        // Focal weight of visual point A. Scales from 0 to 1 with distance
        private double FocalWeight(ReadingPoint readingPointA, ReadingPoint readingPointB)
        {
            double distance = Utils.NormalisedDistance(readingPointA, readingPointB);

            return Math.Tanh((distance - FocalDistanceThreshold) * FocalDistanceCurveHarshness) / 2 + 0.5;
        }

        private double OverlapBonus(ReadingPoint readingPointA, ReadingPoint readingPointB)
        {
            if (readingPointB == null)
            {
                return 1;
            }

            double distance = Utils.NormalisedDistance(readingPointA, readingPointB);

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
            
            if (_currentDiffPoints.Count < 3)   // Not enough visual points to cause rhythmic difficulty
            {
                return 0;
            }

            // Calculate and set rhythmic focal weight of the visual point
            ReadingPointA.RhythmicFocalWeight = RhythmicFocalWeight(ReadingPointA, ReadingPointB, ReadingPointC);

            // TODO: Currently notes that have a back forth where the middle note is a slider cause false positives for timing change since slider bodies are ignored.
            //          To fix, could give ReadingPoints an end point that would be the same as start point for normal circles, but coords of slidertail for sliders

            return RhythmicFocalTotal * ReadingPointA.RhythmicFocalWeight;
        }

        private double RhythmicFocalWeight(ReadingPoint readingPointA, ReadingPoint readingPointB, ReadingPoint readingPointC)
        {
            // Step 1: Determine distance change weight (low distance change = high, decreases and drops off exponentially)
            double distanceAB = Utils.NormalisedDistance(readingPointA, readingPointB);
            double distanceBC = Utils.NormalisedDistance(readingPointB, readingPointC);
            double distanceChangeWeight = DistanceChangeWeight(distanceAB, distanceBC);

            // Step 2: Determine timing change weight (low timing change = low, increases and caps out exponentially)
            double delayAB = readingPointA.Offset - readingPointB.Offset;
            double delayBC = readingPointB.Offset - readingPointC.Offset;
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
    }
}
