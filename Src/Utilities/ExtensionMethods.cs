using System;
using System.Collections.Generic;
using UniLinq;
using UnityEngine;
using Random = System.Random;

namespace Stargate.Utilities
{
    public static class Extensions
    {
        public static Vector3d Clone(this Vector3d vector)
        {
            return new Vector3d(vector.x, vector.y, vector.z);
        }

        public static Vector3 Clone(this Vector3 vector)
        {
            return new Vector3(vector.x, vector.y, vector.z);
        }

        public static double Round(this double value, int digits = 0)
        {
            return Math.Round(value, digits);
        }

        public static int ToBinary(this bool value)
        {
            return value ? 1 : 0;
        }

        public static float CoerceAtMost(this float value, float threshold)
        {
            return value > threshold
                ? threshold
                : value;
        }

        public static float CoerceAtLeast(this float value, float threshold)
        {
            return value < threshold
                ? threshold
                : value;
        }

        public static float Absolute(this float value)
        {
            return Math.Abs(value);
        }

        public static List<T> Shuffle<T>(this IList<T> list)
        {
            var rng = new Random();
            var n = list.Count;
            while (n > 1) {
                n--;
                var k = rng.Next(n + 1);
                (list[k], list[n]) = (list[n], list[k]);
            }

            return list.ToList();
        }
    }
}
