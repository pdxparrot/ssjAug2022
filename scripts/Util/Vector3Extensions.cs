using Godot;

using System;

namespace pdxpartyparrot.ssjAug2022.Util
{
    public static class Vector3Extensions
    {
        public static float ManhattanDistanceTo(this Vector3 v, Vector3 position)
        {
            return Math.Abs(v.x - position.x) + Math.Abs(v.y - position.y) + Math.Abs(v.z - position.z);
        }

        public static Vector3 Perpendicular(this Vector3 v)
        {
            return new Vector3(-v.z, v.y, v.x);
        }
    }
}
