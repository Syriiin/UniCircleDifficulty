using System;
using System.Linq;

using UniCircleTools;
using UniCircleTools.Beatmaps;

namespace UniCircleDifficulty.Skills.Reading
{
    /// <summary>
    /// Skill representing the difficulty of identifying rhythmic and visual patterns in notes
    /// </summary>
    public class Reading : Skill<VisualPoint>
    {
        // Curve constants
        private const double focal_distance_threshold = 100;
        private const double focal_distance_curve_harshness = 0.05;  // Higher = quicker change

        private const double overlap_threshold = 25;
        private const double overlap_curve_harshness = 0.1;

        private const double rhythm_distance_curve_harshness = 7;
        private const double rhythm_delay_curve_harshness = 10;

        // Shortcuts for readability
        private VisualPoint VisualPointA => GetDifficultyPoint(0);
        private VisualPoint VisualPointB => GetDifficultyPoint(1);
        private VisualPoint VisualPointC => GetDifficultyPoint(2);

        private double FocalTotal => _currentDiffPoints.Sum(vp => vp.FocalWeight);
        private double RhythmicFocalTotal => _currentDiffPoints.Sum(vp => vp.RhythmicFocalWeight);

        private double AimReadingWeight => 1;
        private double RhythmicReadingWeight => 1;

        protected override double SkillMultiplier => 0.1;

        public override void ProcessHitObject(HitObject hitObject)
        {
            if (hitObject is Spinner)
            {
                // Spinners are not considered in reading
                return;
            }

            // Construct visual points from hitobject and call ProcessDifficultyPoint with them
            VisualPoint visualPoint = new VisualPoint
            {
                Offset = hitObject.Time / Utils.ModClockRate(_mods),
                ApproachTime = Utils.ModApproachTime(hitObject.Difficulty.AR, _mods) / Utils.ModClockRate(_mods),
                X = hitObject.X,
                Y = hitObject.Y,
                Radius = Utils.ModRadius(hitObject.Difficulty.CS, _mods)
            };

            if (_mods.HasFlag(Mods.HardRock))
            {
                visualPoint.Y = -visualPoint.Y + 384; // Flip notes (even though it technically doesnt matter since EVERYTHING is flipped)
            }

            // TODO: consider reading implications of sliders (create visual points at corners in curve?)

            ProcessDifficultyPoint(visualPoint);
        }

        protected override void UpdateDifficultyPoints(VisualPoint visualPoint)
        {
            // Add visualPoint to currentDiffPoints
            _currentDiffPoints.Add(visualPoint);

            // Remove points that are no longer visible
            double currentTime = visualPoint.Offset - visualPoint.ApproachTime;   // Visual points are processed at the moment they first appear
            _currentDiffPoints.RemoveAll(vp => vp.Offset < currentTime);
        }

        protected override void CalculateDifficulty()
        {
            VisualPoint visualPoint = VisualPointA;

            if (_currentDiffPoints.Count == 1)   // Not enough visual points to cause difficulty
            {
                visualPoint.Difficulty = 0;
                return;
            }

            double aimReading = AimReading() * AimReadingWeight;
            double rhythmicReading = RhythmicReading() * RhythmicReadingWeight;
            double speedBonus = SpeedBonus(visualPoint.ApproachTime);

            visualPoint.Difficulty = speedBonus * (aimReading + rhythmicReading);
        }

        /// <summary>
        /// Difficulty of determining the position and order of aim points
        /// </summary>
        /// <returns>Difficulty as a double</returns>
        private double AimReading()
        {
            // Step 1: Assign point a focal weight
            VisualPointA.FocalWeight = FocalWeight(VisualPointA, VisualPointB);

            // Step 2: Search for nearest note
            VisualPoint nearestPoint = _currentDiffPoints.Take(_currentDiffPoints.Count - 1).OrderBy(vp => Utils.NormalisedDistance(vp, VisualPointA)).FirstOrDefault();

            // Step 3: Calculate overlap bonus
            double overlapBonus = OverlapBonus(VisualPointA, nearestPoint);
            
            return FocalTotal * VisualPointA.FocalWeight * overlapBonus;
        }

        // Focal weight of visual point A. Scales from 0 to 1 with distance
        private static double FocalWeight(VisualPoint visualPointA, VisualPoint visualPointB)
        {
            double distance = Utils.NormalisedDistance(visualPointA, visualPointB);

            return Math.Tanh((distance - focal_distance_threshold) * focal_distance_curve_harshness) / 2 + 0.5;
        }

        private static double OverlapBonus(VisualPoint visualPointA, VisualPoint visualPointB)
        {
            if (visualPointB == null)
            {
                return 1;
            }

            double distance = Utils.NormalisedDistance(visualPointA, visualPointB);

            return Math.Tanh((distance - overlap_threshold) * overlap_curve_harshness) / -2 + 1.5;
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
            VisualPointA.RhythmicFocalWeight = RhythmicFocalWeight(VisualPointA, VisualPointB, VisualPointC);

            // TODO: Currently notes that have a back forth where the middle note is a slider cause false positives for timing change since slider bodies are ignored.
            //          To fix, could give VisualPoints an end point that would be the same as start point for normal circles, but coords of slidertail for sliders

            return RhythmicFocalTotal * VisualPointA.RhythmicFocalWeight;
        }

        private static double RhythmicFocalWeight(VisualPoint visualPointA, VisualPoint visualPointB, VisualPoint visualPointC)
        {
            // Step 1: Determine distance change weight (low distance change = high, decreases and drops off exponentially)
            double distanceAB = Utils.NormalisedDistance(visualPointA, visualPointB);
            double distanceBC = Utils.NormalisedDistance(visualPointB, visualPointC);
            double distanceChangeWeight = DistanceChangeWeight(distanceAB, distanceBC);

            // Step 2: Determine timing change weight (low timing change = low, increases and caps out exponentially)
            double delayAB = visualPointA.Offset - visualPointB.Offset;
            double delayBC = visualPointB.Offset - visualPointC.Offset;
            double delayChangeWeight = DelayChangeWeight(delayAB, delayBC);

            // Step 3: Return diff
            return distanceChangeWeight * delayChangeWeight;
        }

        // Defines how much the distance change between 3 points affects timing reading
        private static double DistanceChangeWeight(double distanceAB, double distanceBC)
        {
            // Domain: [1, infinity]
            // Range: [0, 1]
            // TODO: Contemplate using absolute distances instead of ratio. Since distance changes effects are less the further they are from each other
            double distanceChange = Math.Min(distanceAB, distanceBC) == 0 ? double.PositiveInfinity : Math.Max(distanceAB, distanceBC) / Math.Min(distanceAB, distanceBC);
            return Math.Pow(Math.E, -rhythm_distance_curve_harshness * (distanceChange - 1));
        }

        // Defines how much the delay change between 3 points affects timing reading
        private static double DelayChangeWeight(double delayAB, double delayBC)
        {
            // Domain: [1, infinity]
            // Range: [0, 1]
            double delayChange = Math.Max(delayAB, delayBC) / Math.Min(delayAB, delayBC);
            if (Math.Abs(delayAB - delayBC) < 3)   // Integer millisecond snapping error
            {
                delayChange = 1;
            }
            return -Math.Pow(Math.E, -rhythm_delay_curve_harshness * (delayChange - 1)) + 1;
        }

        private static double SpeedBonus(double approachTime)
        {
            // 2x bonus at ar11
            // 1.082x bonus at ar10.3
            return Math.Pow(Math.E, (-approachTime + 300) / 40) + 1;
        }

        public Reading(Mods mods) : base(mods) { }
    }
}
