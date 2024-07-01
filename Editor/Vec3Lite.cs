using UnityEngine;

namespace CustomMissionUtility
{
    // for nicer looking json
    internal class Vec3Lite
    {
        public static explicit operator Vector3(Vec3Lite vec) => new Vector3(vec.x, vec.y, vec.z);
        public static explicit operator Vec3Lite(Vector3 vec) => new Vec3Lite(vec.x, vec.y, vec.z);

        public float x; public float y; public float z;

        public Vec3Lite(float x, float y, float z) {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }
}
