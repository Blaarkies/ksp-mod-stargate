using System;
using UnityEngine;

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

        public static double Round(this double value)
        {
            return Math.Round(value);
        }

        public static int ToSign(this bool value)
        {
            return value ? 1 : 0;
        }
    }
}
