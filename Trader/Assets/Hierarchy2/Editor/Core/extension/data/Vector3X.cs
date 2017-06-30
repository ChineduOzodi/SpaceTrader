using UnityEngine;

public static class Vector3X {
    public static Vector3 xFixNaN(this Vector3 v) {
        return new Vector3(float.IsNaN(v.x) ? 0 : v.x, float.IsNaN(v.y) ? 0 : v.y, float.IsNaN(v.z) ? 0 : v.z);
    }

    public static Vector3 xLerp(this Vector3 stValue, Vector3 edValue, float t) {
        return new Vector3(
            stValue.x + (edValue.x - stValue.x)*t, stValue.y + (edValue.y - stValue.y)*t,
            stValue.z + (edValue.z - stValue.z)*t);
    }
}