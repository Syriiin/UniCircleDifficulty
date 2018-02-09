using System;

using UniCircleTools;

namespace UniCircle.Difficulty.Standard.Skills
{
    static class Utils
    {
        /// <summary>
        /// Calculate normalised distance of 2 circles with circle size 52
        /// </summary>
        /// <param name="circleA">1st circle</param>
        /// <param name="circleB">2nd circle</param>
        /// <returns>Normalised distance</returns>
        public static double NormalisedDistance(ICircle circleA, ICircle circleB)
        {
            // Average CS (to support possible lazer variable CS)
            double avgRadius = (circleB.Radius + circleA.Radius) / 2;
            // Ratio of distance to CS
            double distanceRatio = Distance(circleB, circleA) / avgRadius;
            // Normalised distance at radius 52
            return distanceRatio * 52;
        }

        public static double Distance(ICircle circleA, ICircle circleB)
        {
            return Distance(circleA.X, circleA.Y, circleB.X, circleB.Y);
        }
        public static double Distance(double x1, double y1, double x2, double y2)
        {
            // Pythag
            return Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
        }

        /// <summary>
        /// Calculate the angle formed by 3 <see cref="AimPoint"/>s in radians
        /// </summary>
        /// <param name="circleA">1st point</param>
        /// <param name="circleB">2nd point</param>
        /// <param name="circleC">3rd point</param>
        /// <returns>Inner angle in radians</returns>
        public static double Angle(ICircle circleA, ICircle circleB, ICircle circleC)
        {
            return Angle(circleA.X, circleA.Y, circleB.X, circleB.Y, circleC.X, circleC.Y);
        }
        public static double Angle(double x1, double y1, double x2, double y2, double x3, double y3)
        {
            // Cosine rule: Cos(C) = a^2 + b^2 - c^2 / 2ab
            // where    a = distance from A to B
            //          b = distance from B to C
            //          c = distance from C to A
            double a = Distance(x1, y1, x2, y2);
            double b = Distance(x2, y2, x3, y3);
            double c = Distance(x3, y3, x1, y1);
            return Math.Acos((Math.Pow(a, 2) + Math.Pow(b, 2) - Math.Pow(c, 2)) / (2 * a * b));
        }

        /// <summary>
        /// Speed effect of passed mods
        /// </summary>
        /// <param name="mods">Mods to apply</param>
        /// <returns>Clock rate</returns>
        public static double ModClockRate(Mods mods)
        {
            if (mods.HasFlag(Mods.DoubleTime))
            {
                return 1.5;
            }
            else if (mods.HasFlag(Mods.HalfTime))
            {
                return 0.75;
            }

            return 1;
        }

        /// <summary>
        /// Radius in osu!px with passed mods
        /// </summary>
        /// <param name="cs">Original CS</param>
        /// <param name="mods">Mods to apply</param>
        /// <returns>Radius in osu!px</returns>
        public static double ModRadius(double cs, Mods mods)
        {
            if (mods.HasFlag(Mods.HardRock))
            {
                cs *= 1.3;
            }
            else if (mods.HasFlag(Mods.Easy))
            {
                cs *= 0.5;
            }

            return 64 * (1 - 0.7 * (cs - 5) / 5) / 2;
        }

        public static double ModApproachTime(double ar, Mods mods)
        {
            if (mods.HasFlag(Mods.HardRock))
            {
                ar *= 1.4;
            }
            else if (mods.HasFlag(Mods.Easy))
            {
                ar *= 0.5;
            }

            if (ar <= 5)
            {
                return 1800 - (ar * 120);
            }
            else
            {
                return 1950 - (ar * 150);
            }
        }
    }
}
