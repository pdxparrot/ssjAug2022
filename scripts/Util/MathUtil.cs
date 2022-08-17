using Godot;

using System;

namespace pdxpartyparrot.ssjAug2022.Util
{
    public static class MathUtil
    {
        // this can go away when we have System.Math.Clamp
        public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
        {
            if(val.CompareTo(min) < 0) {
                return min;
            }
            if(val.CompareTo(max) > 0) {
                return max;
            }
            return val;
        }
    }
}
