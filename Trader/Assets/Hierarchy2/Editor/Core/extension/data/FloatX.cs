using UnityEngine;

public static class FloatX {
    public static float xLerp(this float stVal, float edVal, float t) {
        //return stVal + (edVal - stVal) * t;
        return Mathf.Lerp(stVal, edVal, t);
    }

    public static float xFixNaN(this float val, float defaultVal = 0f) { return float.IsNaN(val) ? defaultVal : val; }

    public static float xFixInfinity(this float val, float niVal, float piVal) {
        if (!float.IsInfinity(val)) return val;
        return float.IsPositiveInfinity(val) ? piVal : niVal;
    }

    internal static float xLerpStep(this float from, float to, float minDelta = 0.5f, float frac = 0.1f) {
        var d = to - from;
        if (d >= -minDelta && d <= minDelta) return to;
        return Mathf.Lerp(from, to, frac);
    }
}