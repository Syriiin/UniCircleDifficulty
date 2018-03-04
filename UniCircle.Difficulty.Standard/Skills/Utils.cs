using System;

using UniCircleTools;

namespace UniCircle.Difficulty.Standard.Skills
{
    /// <summary>
    /// Collection of utility functions used by several skills
    /// </summary>
    internal static class Utils
    {
        /// <summary>
        /// Distance between the centers of 2 circles
        /// </summary>
        /// <param name="circleA">First circle</param>
        /// <param name="circleB">Second circle</param>
        public static double Distance(ICircle circleA, ICircle circleB)
        {
            return Distance(circleA.X, circleA.Y, circleB.X, circleB.Y);
        }
        /// <summary>
        /// Distance between 2 points
        /// </summary>
        /// <param name="x1">First X</param>
        /// <param name="y1">First Y</param>
        /// <param name="x2">Second X</param>
        /// <param name="y2">Second Y</param>
        public static double Distance(double x1, double y1, double x2, double y2)
        {
            // Pythag
            return Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
        }

        /// <summary>
        /// Calculate the angle formed by 3 <see cref="ICircle"/>s in radians
        /// </summary>
        /// <param name="circleA">1st point</param>
        /// <param name="circleB">2nd point</param>
        /// <param name="circleC">3rd point</param>
        /// <returns>Inner angle in radians</returns>
        public static double Angle(ICircle circleA, ICircle circleB, ICircle circleC)
        {
            return Angle(circleA.X, circleA.Y, circleB.X, circleB.Y, circleC.X, circleC.Y);
        }
        /// <summary>
        /// Calculate the angle formed by 3 points in radians
        /// </summary>
        /// <param name="x1">First X</param>
        /// <param name="y1">First Y</param>
        /// <param name="x2">Second X</param>
        /// <param name="y2">Second Y</param>
        /// <param name="x3">Third X</param>
        /// <param name="y3">Third Y</param>
        /// <returns>Inner angle</returns>
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
        /// Speed multiplier with passed mods
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

        /// <summary>
        /// Approach time in ms with passed mods
        /// </summary>
        /// <param name="ar"></param>
        /// <param name="mods"></param>
        /// <returns>Approach rate in milliseconds</returns>
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
