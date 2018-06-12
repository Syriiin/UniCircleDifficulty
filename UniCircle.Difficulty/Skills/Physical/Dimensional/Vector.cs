using System;
using System.Linq;

namespace UniCircle.Difficulty.Skills.Physical.Dimensional
{
    public class Vector
    {
        private double[] _components;

        public int Dimensions => _components.Length;

        // operator overloads

        public Vector(double[] components)
        {
            _components = components;
        }
        public Vector(double x)
        {
            _components = new[] {x};
        }
        public Vector(double x, double y)
        {
            _components = new[] {x, y};
        }
        public Vector(double x, double y, double z)
        {
            _components = new[] {x, y, z};
        }

        // Vector length (magnitude) equals the square root of the sum of its squared components (pythagoras)
        public double Length => Math.Sqrt(_components.Sum(c => Math.Pow(c, 2)));

        // Unit vector (magnitude of 1) is found by dividing the components by the vector magnitude
        public Vector UnitVector
        {
            get
            {
                if (Length == 0)
                {
                    return new Vector(_components); // zero vectors cant have unit vectors so lets just return another zero vector
                }
                return new Vector(_components.Select(c => c / Length).ToArray());
            }
        }

        public static Vector operator +(Vector v1, Vector v2)
        {
            if (v1.Dimensions != v2.Dimensions)
            {
                throw new Exception("Vector dimension mismatch");
            }

            // Vector addition is just the addition of their components
            return new Vector(v1._components.Zip(v2._components, (c1, c2) => c1 + c2).ToArray());
        }

        public static Vector operator -(Vector v1, Vector v2)
        {
            if (v1.Dimensions != v2.Dimensions)
            {
                throw new Exception("Vector dimension mismatch");
            }

            // Vector subtraction is just the subtraction of their components
            return new Vector(v1._components.Zip(v2._components, (c1, c2) => c1 - c2).ToArray());
        }

        public static Vector operator -(Vector v)
        {
            // Vector negation is just the negating its components
            return new Vector(v._components.Select(c => -c).ToArray());
        }
    }
}
