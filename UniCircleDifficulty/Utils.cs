using System;

using UniCircleTools.Beatmaps;

namespace UniCircleDifficulty
{
    static class Utils
    {
        public static double Distance(HitObject hitObject1, HitObject hitObject2)
        {
            // Pythag
            double distance = Math.Sqrt(Math.Pow(hitObject1.X - hitObject2.X, 2) + Math.Pow(hitObject1.Y - hitObject2.Y, 2));
            return distance;
        }

        /// <summary>
        /// Return the angle formed by 3 hit objects in radians
        /// </summary>
        /// <param name="hitObjectA">1st hit object</param>
        /// <param name="hitObjectB">2nd hit object</param>
        /// <param name="hitObjectC">3rd hit object</param>
        /// <returns>Inner angle</returns>
        public static double Angle(HitObject hitObjectA, HitObject hitObjectB, HitObject hitObjectC)
        {
            // Cosine rule: Cos(C) = a^2 + b^2 - c^2 / 2ab
            // where    a = distance from A to B
            //          b = distance from B to C
            //          c = distance from C to A
            double a = Distance(hitObjectA, hitObjectB);
            double b = Distance(hitObjectB, hitObjectC);
            double c = Distance(hitObjectC, hitObjectA);
            return Math.Acos((Math.Pow(a, 2) + Math.Pow(b, 2) - Math.Pow(c, 2)) / (2 * a * b));
        }
    }
}
